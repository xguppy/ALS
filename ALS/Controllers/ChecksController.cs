using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using ALS.CheckModule.Compare;
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
        public async Task<IActionResult> Check([FromHeader] string sourceCode, [FromHeader] int variantId, [FromHeader] int userId)
        {
            var userIdentifier = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var variant = await _db.Variants.Include(var => var.LaboratoryWork).FirstOrDefaultAsync(var => var.VariantId == variantId);
            if (userIdentifier == userId && variant != null)
            {
                var solution =
                    await _db.Solutions.FirstOrDefaultAsync(sol => sol.VariantId == variantId && sol.UserId == userId && sol.IsSolved);
                sourceCode = HttpUtility.UrlDecode(sourceCode);
                if (solution == null)
                {
                    solution = new Solution {SourceCode = sourceCode, VariantId = variantId, UserId = userId, IsSolved = true, SendDate = DateTime.Now};
                    var sourceCodeFile = Path.Combine(Environment.CurrentDirectory, "sourceCodeUser",
                        $"{ProcessCompiler.CreatePath(variant.LaboratoryWorkId, variantId)}.cpp");
                    
                    using (var fileWrite = new StreamWriter(sourceCodeFile))
                    {
                        await fileWrite.WriteAsync(sourceCode);
                    }

                    var programFileUser =
                        Path.Combine(Environment.CurrentDirectory, "executeUser",
                            $"{ProcessCompiler.CreatePath(variant.LaboratoryWorkId, variantId)}.exe");
                    
                    var programFileModel = new Uri((await _db.Variants.FirstOrDefaultAsync(var => var.VariantId == variantId)).LinkToModel).AbsolutePath;
                    
                    var compiler = new ProcessCompiler(sourceCodeFile, programFileUser);
                    var isCompile = await Task.Run(() => compiler.Execute(60000));
                    
                    if (isCompile != true)
                    {
                        var lastSol = await _db.Solutions.OrderBy(sol => sol.SolutionId).LastOrDefaultAsync(sol => sol.UserId == userId && sol.VariantId == variantId) ??
                                      solution;
                        lastSol.IsSolved = false;
                        lastSol.CompilerFailsNumbers++;
                        _db.Solutions.Update(lastSol);
                        await _db.SaveChangesAsync();
                        return BadRequest(compiler.CompileState);
                    }

                    System.IO.File.Delete(sourceCodeFile);
                    
                    var gen = new GenFunctions();
                    
                    var inputDatas = gen.GetTestsFromJson(variant.InputDataRuns);
                    
                    var constrains = JsonConvert.DeserializeObject<CompareData>(
                        variant.LaboratoryWork.Constraints,
                        new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Populate});
                    await _db.Solutions.AddAsync(solution);
                    await _db.SaveChangesAsync();
                    var errorsRuns = default(int);
                    foreach (var elem in inputDatas)
                    {
                        var cmp = new CompareModel(programFileModel, programFileUser, elem);
                        var dataRun = await cmp.CompareAsync(constrains.Time, constrains.Memory);
                        var testRun = new TestRun
                        {
                            InputData = elem.ToArray(),
                            OutputData = cmp.UserOutput.ToArray(), 
                            ResultRun = JsonConvert.SerializeObject(dataRun),
                            SolutionId = solution.SolutionId
                        };
                        await _db.TestRuns.AddAsync(testRun);
                        
                        if (dataRun.IsCorrect != true)
                        {
                            ++errorsRuns;
                            solution.IsSolved = false;
                        }
                    }
                    System.IO.File.Delete(programFileUser);
                    await _db.SaveChangesAsync();
                    return Ok($"{inputDatas.Count - errorsRuns}/{inputDatas.Count} runs complete");

                }
                return Ok("Solution is solved");
            }

            return BadRequest("Not Privilege");
        }
    }
}