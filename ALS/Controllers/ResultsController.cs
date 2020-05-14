using System;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetSolutions([FromHeader] int labId, [FromHeader] int userId)
            => Ok(await Task.Run(() => _db.Solutions.Include(sol => sol.AssignedVariant)
                .ThenInclude(aw => aw.Variant)
                .Where(sol => sol.AssignedVariant.UserId == userId)
                .Where(sol => sol.AssignedVariant.Variant.LaboratoryWorkId == labId)));

        [HttpGet]
        public async Task<IActionResult> GetTestRuns([FromHeader]int solutionId)
            => Ok(await Task.Run(() => _db.Solutions.Where(sol => sol.SolutionId == solutionId).Select(sol => sol.TestRuns)));

        [HttpGet]
        public async Task<FileResult> GetCode([FromQuery]int solutionId)
        {
            var sourceCodePath = await Task.Run(() =>
                _db.Solutions.Where(sol => sol.SolutionId == solutionId).Select(sol => sol.SourceCode).Single());
            var dirInfo = new DirectoryInfo(sourceCodePath);
            var fileName = $"{dirInfo.Parent.Name}_{dirInfo.Parent.Parent.Name}_{dirInfo.Name}.zip";
            var resultDirectory = Path.Combine(Environment.CurrentDirectory, "tmp", fileName);
            ZipFile.CreateFromDirectory(sourceCodePath, resultDirectory);
            var dataBytes = await System.IO.File.ReadAllBytesAsync(resultDirectory);
            System.IO.File.Delete(resultDirectory);
            return File(dataBytes, "application/zip", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetResultLab(int labId, int userId) =>
        Ok(await Task.Run(() => _db.AssignedVariants
                .Where(av => av.Variant.LaboratoryWorkId == labId && av.UserId == userId)
                .Select(av => new {av.Variant.VariantNumber, av.AssignDateTime, av.Solutions.FirstOrDefault(sol => sol.IsSolved).SendDate,av.Variant.LaboratoryWork.Constraints, av.Mark})));
            
        
    }
}