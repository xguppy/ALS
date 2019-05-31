using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [Produces("application/json")]
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
        public async Task<IActionResult> Check([FromBody]string sourceCode, [FromHeader]int solutionId)
        {
            var userIdentifier = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var solution =
                await _db.Solutions.Include(sol => sol.Variant).ThenInclude(var => var.LaboratoryWork).Include(sol => sol.TestRuns).FirstOrDefaultAsync(sol => sol.SolutionId == solutionId);
            if (solution != null && userIdentifier != solution.UserId)
            {
                if (solution.IsSolved)
                {
                    return Ok("Solution is solve");
                }
                solution.SourceCode = sourceCode;
                solution.SendDate = DateTime.Now;
                var sourceCodeFile = Path.Combine(Environment.CurrentDirectory,"sourceCodeUser", $"{ProcessCompiler.CreatePath(solution.Variant.LaboratoryWorkId, solution.VariantId)}.cpp");
                using (var fileWrite = new StreamWriter(sourceCodeFile))
                {
                    await fileWrite.WriteAsync(sourceCode);
                }
                var programFileUser =
                    Path.Combine(Environment.CurrentDirectory,"executeUser", $"{ProcessCompiler.CreatePath(solution.Variant.LaboratoryWorkId, solution.VariantId)}.exe");
                var programFileModel =
                    Path.Combine(Environment.CurrentDirectory,"executeModel", $"{ProcessCompiler.CreatePath(solution.Variant.LaboratoryWorkId, solution.VariantId)}.exe");
                var compiler = new ProcessCompiler(sourceCodeFile, programFileUser);
                var isCompile = await Task.Run(() =>  compiler.Execute(60000));
                if (isCompile != true)
                {
                    return BadRequest(await compiler.Error.ReadToEndAsync());
                }
                System.IO.File.Delete(sourceCodeFile);
                var gen = new GenFunctions();
                var inputDatas = gen.GetTestsFromJson(solution.Variant.InputDataRuns);
                var constrains = JsonConvert.DeserializeObject<CompareData>(solution.Variant.LaboratoryWork.Constraints, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });
                _db.TestRuns.RemoveRange(solution.TestRuns);
                foreach (var elem in inputDatas)
                {
                    var cmp = new CompareModel(programFileModel, programFileUser, elem);
                    var dataRun = await cmp.Compare(constrains.Time, constrains.Memory);
                    var testRun = new TestRun {Solution = solution, InputData = elem.ToArray(), OutputData = cmp.UserOutput.ToArray(), ResultRun = JsonConvert.SerializeObject(dataRun)};
                    await _db.TestRuns.AddAsync(testRun);
                    if (dataRun.IsCorrect != true)
                    {
                        solution.IsSolved = false;
                    }
                }
                _db.Solutions.Update(solution);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}