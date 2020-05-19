using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ALS.CheckModule.Compare.DataStructures;
using ALS.DTO;
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
        public async Task<IActionResult> GetResultLab([FromHeader] List<int> labId, [FromHeader] List<int> userId)
        {
            var result = new List<List<VariantResultDTO>>();
            for (var userIterator = 0; userIterator < userId.Count; ++userIterator)
            {
                var resRow = new List<VariantResultDTO>();
                for (var labIterator = 0; labIterator < labId.Count; labIterator++)
                {
                    var row = await Task.Run(() => _db.AssignedVariants
                        .Where(av => userId[userIterator] == av.UserId && labId[labIterator] == av.Variant.LaboratoryWorkId)
                        .Select(av => new VariantResultDTO
                        {
                            VariantNumber = av.Variant.VariantNumber, AssignDateTime = av.AssignDateTime,
                            SendDate = av.Solutions.FirstOrDefault(sol => sol.IsSolved).SendDate,
                            Evaluation = av.Variant.LaboratoryWork.Evaluation, Mark = av.Mark
                        }).FirstOrDefault());
                    resRow.Add(row);
                }
                result.Add(resRow);
            }
            return Ok(result);
        }
    }
}