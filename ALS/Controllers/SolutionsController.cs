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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class SolutionsController : Controller
    {
        private readonly ApplicationContext _db;

        public SolutionsController(ApplicationContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int varId)
        {
            if (await _db.Variants.Where(w => w.VariantId == varId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Solutions.Where(v => v.VariantId == varId).Select(v => new { v.SendDate, v.SourceCode, v.IsSolved  }).ToList()));
            }

            return BadRequest();
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int solutionId)
        {
            
            var variant = await _db.Solutions.FirstOrDefaultAsync(v => v.VariantId == solutionId);
            if (variant != null)
            {
                return Ok(await _db.Solutions.Where(v => v.SolutionId == solutionId).Select(v => new { v.SendDate, v.SourceCode, v.IsSolved  }).FirstAsync());
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SolutionDTO model)
        {
            var solution = 
                new Solution { SendDate = model.SendDate, SourceCode = model.SourceCode,  IsSolved = model.IsSolved, UserId = model.UserId, VariantId = model.VariantId};
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
                    solutionUpdate.UserId = model.UserId;
                    solutionUpdate.VariantId = model.VariantId;
                    _db.Solutions.Update(solutionUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound();
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
            return NotFound();
        }
    }
}