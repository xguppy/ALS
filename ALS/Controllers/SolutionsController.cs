using System.Linq;
using System.Threading.Tasks;
using ALS.DTO;
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
                return Ok(await Task.Run(() => _db.Solutions.Include(sol => sol.AssignedVariant).Where(sol => sol.AssignedVariant.VariantId == varId).Select(v => new { v.SendDate, v.CompilerFailsNumbers, v.SourceCode, v.IsSolved  }).ToList()));
            }

            return BadRequest("Not Privilege");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetByUser(int varId, int userId)
        {
            if (await _db.Variants.Where(w => w.VariantId == varId).FirstOrDefaultAsync() != null && await _db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Solutions.Include(sol => sol.AssignedVariant).Where(sol => sol.AssignedVariant.VariantId == varId && sol.AssignedVariant.UserId == userId).Select(v => new { v.SendDate, v.CompilerFailsNumbers, v.SourceCode, v.IsSolved  }).ToList()));
            }

            return BadRequest("Not Privilege");
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int solutionId)
        {
            
            var variant = await _db.Solutions.FirstOrDefaultAsync(sol => sol.SolutionId == solutionId);
            if (variant != null)
            {
                return Ok(await _db.Solutions.Where(v => v.SolutionId == solutionId).Select(v => new { v.SendDate, v.CompilerFailsNumbers, v.SourceCode, v.IsSolved  }).FirstAsync());
            }
            return NotFound("Solution not found");
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SolutionDTO model)
        {
            var solution = 
                new Solution { SendDate = model.SendDate, SourceCode = model.SourceCode,  IsSolved = model.IsSolved, AssignedVariantId = model.AssignedVariantId};
            try
            {
                await _db.Solutions.AddAsync(solution);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SolutionDTO model, int solutionId)
        {
            var solutionUpdate = await _db.Solutions.FirstOrDefaultAsync(solution => solution.SolutionId == solutionId);
            if (solutionUpdate != null)
            {
                try
                {
                    solutionUpdate.SendDate = model.SendDate;
                    solutionUpdate.SourceCode = model.SourceCode;
                    solutionUpdate.IsSolved = model.IsSolved;
                    solutionUpdate.AssignedVariantId = model.AssignedVariantId;
                    _db.Solutions.Update(solutionUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound("Solution not found");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int solutionId)
        {
            var solutionUpdate = await _db.Solutions.FirstOrDefaultAsync(solution => solution.SolutionId == solutionId);
            if (solutionUpdate != null)
            {
                try
                {
                    _db.Solutions.Remove(solutionUpdate);
                    await _db.SaveChangesAsync();
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