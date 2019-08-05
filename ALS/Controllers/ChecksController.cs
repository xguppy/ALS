using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using ALS.CheckModule.Compare;
using ALS.CheckModule.Compare.DataStructures;
using ALS.CheckModule.Processes;
using ALS.EntityСontext;
using Generator.MainGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Student")]
    public class ChecksController : Controller
    {
        private readonly ApplicationContext _db;

        public ChecksController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Check([FromHeader] string sourceCode, [FromHeader] int variantId)
        {
            //Получим идентификатор юзера из его сессии
            var userIdentifier = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            //Проверим что вариант назначен пользователю
            var assignedVar =
                await _db.AssignedVariants.Include(aw => aw.Variant).ThenInclude(var => var.LaboratoryWork).FirstOrDefaultAsync(av =>
                    av.UserId == userIdentifier && av.VariantId == variantId);
            if (assignedVar != null)
            {
                //Возможно пользователь уже решил вариант
                var solution =
                    await _db.Solutions.FirstOrDefaultAsync(sol => sol.AssignedVariant == assignedVar && sol.IsSolved);

                if (solution == null)
                {
                    //Если не решил, то декодируем его код
                    sourceCode = HttpUtility.UrlDecode(sourceCode);
                    
                    //Создаём новое решение
                    solution = new Solution {SourceCode = sourceCode, AssignedVariant = assignedVar, IsSolved = true, SendDate = DateTime.Now};
                    
                    //Сохраним его код
                    var sourceCodeFile = Path.Combine(Environment.CurrentDirectory, "sourceCodeUser",
                        $"{ProcessCompiler.CreatePath(assignedVar.Variant.LaboratoryWorkId, variantId)}.cpp");
                    using (var fileWrite = new StreamWriter(sourceCodeFile))
                    {
                        await fileWrite.WriteAsync(sourceCode);
                    }
                    
                    //Возьмём пути для исполняемых файлов
                    var programFileUser =
                        Path.Combine(Environment.CurrentDirectory, "executeUser",
                            $"{ProcessCompiler.CreatePath(assignedVar.Variant.LaboratoryWorkId, variantId)}.exe");
                    var programFileModel = new Uri((await _db.Variants.FirstOrDefaultAsync(var => var.VariantId == variantId)).LinkToModel).AbsolutePath;
                    
                    //Скомпилируем программу пользователя
                    var compiler = new ProcessCompiler(sourceCodeFile, programFileUser);
                    var isCompile = await compiler.Execute(60000);
                    
                    //Удалим ненужный файл исходного кода пользоватля
                    System.IO.File.Delete(sourceCodeFile);

                    //Если не скомпилировалась заносим, то в последнее решение добавим информацию что программа пользователя не была скомпилированна
                    if (isCompile != true)
                    {
                        var lastSol = await _db.Solutions.OrderBy(sol => sol.SolutionId).LastOrDefaultAsync(sol => sol.AssignedVariant == assignedVar) ??
                                      solution;
                        lastSol.IsSolved = false;
                        lastSol.CompilerFailsNumbers++;
                        _db.Solutions.Update(lastSol);
                        await _db.SaveChangesAsync();
                        return BadRequest(compiler.CompileState);
                    }
                    
                    //Получим входные данные для задачи
                    var gen = new GenFunctions();
                    var inputDatas = gen.GetTestsFromJson(assignedVar.Variant.InputDataRuns);
                    
                    //Получим её ограничения
                    var constrains = JsonConvert.DeserializeObject<Constrains>(
                        assignedVar.Variant.LaboratoryWork.Constraints,
                        new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Populate});
                    
                    //Прогоним по тестам
                    List<ResultRun> resultTests;
                    try
                    {
                        var verification = new Verification(programFileUser, programFileModel, constrains);
                        resultTests = await verification.RunTests(inputDatas);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                    finally
                    {
                        //В любом случае удалим ненужный исполняемый файл
                        System.IO.File.Delete(programFileUser);
                    }

                    //Сохраним сейчас чтобы добавить тестовые прогоны в БД
                    await _db.Solutions.AddAsync(solution);
                    await _db.SaveChangesAsync();
                    
                    foreach (var result in resultTests)
                    {
                        
                        var testRun = new TestRun
                        {
                            InputData = result.Input.ToArray(),
                            OutputData = result.Output.ToArray(), 
                            ResultRun = JsonConvert.SerializeObject(new { result.Time, result.Memory, result.IsCorrect, result.Comment }),
                            SolutionId = solution.SolutionId
                        };
                        await _db.TestRuns.AddAsync(testRun);
                        
                        if (solution.IsSolved && result.IsCorrect != true)
                        {
                            solution.IsSolved = false;
                        }
                    }
                    await _db.SaveChangesAsync();
                    return Ok($"{resultTests.Count(rt => rt.IsCorrect)} / {resultTests.Count} runs complete");
                }
                return Ok("Solution is solved");
            }
            return BadRequest("Not Privilege");
        }
    }
}