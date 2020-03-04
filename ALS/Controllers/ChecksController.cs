using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ALS.CheckModule.Compare;
using ALS.CheckModule.Compare.DataStructures;
using ALS.CheckModule.Processes;
using ALS.EntityСontext;
using Generator.MainGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Check(IFormFileCollection uploadedSources, [FromHeader] int variantId)
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
                    //Получим метод оценки
                    var evaluation = assignedVar.Variant.LaboratoryWork.Evaluation;
                    //Если не решил, то создадим нужные директории
                    var solutionDirectory = CreateDirectoriesSources(assignedVar.Variant.VariantId, variantId, userIdentifier);
                    //Cохраним его код в директорию пользователя
                    await SaveSources(solutionDirectory, uploadedSources);
                    //Создаём новое решение
                    solution = new Solution {AssignedVariant = assignedVar, IsSolved = true, SendDate = DateTime.Now, SourceCode = solutionDirectory };

                    //Возьмём пути для исполняемых файлов
                    var programFileUser = CreateExecuteDirectories(assignedVar.Variant.VariantId, variantId, userIdentifier);

                    var programFileModel = new Uri((await _db.Variants.FirstOrDefaultAsync(var => var.VariantId == variantId)).LinkToModel).AbsolutePath;
                    
                    //Скомпилируем программу пользователя
                    var compiler = new ProcessCompiler(solutionDirectory, programFileUser);
                    var isCompile = await compiler.Execute(10000);

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

                    var countCompleteTest = resultTests.Count(rt => rt.IsCorrect);
                    var currMark = assignedVar.Mark;
                    Rate(evaluation, countCompleteTest, resultTests.Count, ref currMark);
                    assignedVar.Mark = currMark;
                    await _db.SaveChangesAsync();
                    //Выведем количество верных тестовых прогонов и комментарии к ним
                    return Ok($"{countCompleteTest} / {resultTests.Count} тестов пройдено.{FormattingResultLog(resultTests)}");
                }
                return Ok("Задача уже решена");
            }
            return BadRequest("Нет доступа");
        }
        
        private static string CreateDirectoriesSources(int lwId, int variantId, int userId)
        {
            var userDirectory = Path.Combine(Environment.CurrentDirectory, "sourceCodeUser", userId.ToString());
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }
            //Получим директорию решения пользователя
            var taskDirectory = Path.Combine(userDirectory, ProcessCompiler.CreatePath(lwId, variantId));
            if (!Directory.Exists(taskDirectory))
            {
                Directory.CreateDirectory(taskDirectory);
            }
            //Посмотрим на все его попытки
            var numberLastSolution = 0;
            var directoriesSolutions = Directory.GetDirectories(taskDirectory);
            if (directoriesSolutions.Length != 0)
            {
                numberLastSolution = directoriesSolutions.Max(dir => int.TryParse(Path.GetFileName(dir), out int res) ? res : 0);
            }
            //И создадим папку с новым решением
            var solutionDirectory = Path.Combine(taskDirectory, (numberLastSolution + 1).ToString());
            Directory.CreateDirectory(solutionDirectory);
            
            return solutionDirectory;
        }

        private static string CreateExecuteDirectories(int lwId, int variantId, int userId)
        {
            var userDirectory = Path.Combine(Environment.CurrentDirectory, "executeUser", userId.ToString());
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }
            //Получим директорию решения пользователя
            var taskDirectory = Path.Combine(userDirectory, ProcessCompiler.CreatePath(lwId, variantId));
            if (!Directory.Exists(taskDirectory))
            {
                Directory.CreateDirectory(taskDirectory);
            }

            return Path.Combine(taskDirectory, $"{ProcessCompiler.CreatePath(lwId, variantId)}.exe");
        }
        
        private static async Task SaveSources(string directorySave, IFormFileCollection sources)
        {
            //Сохраним все файлы в директорию пользовательского решения
            foreach (var file in sources)
            {
                using (var fileStream = new FileStream(Path.Combine(directorySave, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
        }
        
        private static string FormattingResultLog(List<ResultRun> results)
        {
            var testsLog = new StringBuilder();
            for (var i = 0; i < results.Count; i++)
            {
                testsLog.Append($"\nТест {i + 1}: {results[i].Comment}");
            }
            return testsLog.ToString();
        }

        private static void Rate(Evaluation evaluation, int testComplete, int sumTest, ref int currentMark)
        {
            switch (evaluation)
            {
                case Evaluation.Strict:
                    currentMark = sumTest == testComplete ? 1 : 0;
                    break;
                case Evaluation.NotStrict:
                    currentMark = testComplete;
                    break;
                case Evaluation.Penalty:
                    currentMark -= sumTest - testComplete;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(evaluation), evaluation, "Выбранная стратегия оценивания отсутствует");
            }
        }
    }
}