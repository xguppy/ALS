using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ALS.CheckModule.Compare;
using ALS.CheckModule.Processes;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                await _db.Solutions.Include(sol => sol.Variant).FirstOrDefaultAsync(sol => sol.SolutionId == solutionId);
            if (solution != null && userIdentifier != solution.UserId)
            {
                solution.SourceCode = sourceCode;
                solution.SendDate = DateTime.Now;
                var sourceCodeFile = Path.Combine(Environment.CurrentDirectory,"sourceCodeUser", $"{ProcessCompiler.CreatePath(solution.Variant.LaboratoryWorkId, solution.VariantId)}.cpp");
                using (var fileWrite =
                    new StreamWriter(sourceCodeFile))
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
                var inputDataJson = solution.Variant.InputDataRuns;
            }
            return BadRequest();
        }
    }
}