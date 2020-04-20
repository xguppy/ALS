using System;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class ResultsController : Controller
    {
        private readonly ApplicationContext _db;

        public ResultsController(ApplicationContext db)
        {
            _db = db;
        }
        
        
        [HttpGet]
        public async Task<IActionResult> GetTestRuns([FromHeader]int solutionId)
        {
            return Ok(await Task.Run(() => _db.Solutions.Where(sol => sol.SolutionId == solutionId).Select(sol => sol.TestRuns)));
        }

        [HttpGet]
        public async Task<FileResult> GetCode([FromQuery]int solutionId)
        {
            throw new NotImplementedException();
            /*var sourceCodePath = await Task.Run(() =>
                _db.Solutions.Where(sol => sol.SolutionId == solutionId).Select(sol => sol.SourceCode).Single());
            var fileName = $"{Directory}";
            var resultDirectory = Path.Combine(Environment.CurrentDirectory, "tmp", fileName);
            ZipFile.CreateFromDirectory(sourceCodePath, resultDirectory);
            return File();*/
        }
    }
}