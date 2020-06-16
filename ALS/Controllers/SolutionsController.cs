using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class SolutionsController : Controller
    {
        private readonly ApplicationContext _db;

        public SolutionsController(ApplicationContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] int varId)
        {
            if (await _db.Variants.Where(w => w.VariantId == varId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Solutions.Include(sol => sol.AssignedVariant).Where(sol => sol.AssignedVariant.VariantId == varId).Select(v => new { v.SendDate, v.IsCompile, v.SourceCode, v.IsSolved  }).ToList()));
            }

            return BadRequest("Not Privilege");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetByUser([FromHeader] int assignedVariantId)
            => Ok(await Task.Run(() => _db.Solutions.Where(sol => sol.AssignedVariantId == assignedVariantId).Select(v => new {v.SolutionId, v.SendDate, v.IsCompile, v.IsSolved  })));

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] int solutionId)
        {
            
            var variant = await _db.Solutions.FirstOrDefaultAsync(sol => sol.SolutionId == solutionId);
            if (variant != null)
            {
                return Ok(await _db.Solutions.Where(v => v.SolutionId == solutionId).Select(v => new { v.SendDate, v.IsCompile, v.SourceCode, v.IsSolved  }).FirstAsync());
            }
            return NotFound("Solution not found");
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] int solutionId)
        {
            var solutionUpdate = await _db.Solutions.FirstOrDefaultAsync(solution => solution.SolutionId == solutionId);
            if (solutionUpdate != null)
            {
                try
                {
                    var deleteTestRuns = _db.TestRuns.Where(tr => tr.SolutionId == solutionId);
                    _db.TestRuns.RemoveRange(deleteTestRuns);
                    _db.Solutions.Remove(solutionUpdate);
                    await _db.SaveChangesAsync();
                    Directory.Delete(solutionUpdate.SourceCode, true);
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound("Solution not found");
        }
    }
}