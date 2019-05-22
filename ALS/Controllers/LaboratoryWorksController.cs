using System;
using System.Linq;
using System.Security.Claims;
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
    public class LaboratoryWorksController : Controller
    {
        private readonly ApplicationContext _db;

        public LaboratoryWorksController(ApplicationContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.Run(() => _db.LaboratoryWorks
                .Select(laboratoryWork => new { laboratoryWork.TemplateLaboratoryWorkId, laboratoryWork.Name, laboratoryWork.Description, laboratoryWork.Evaluation, laboratoryWork.Cipher, laboratoryWork.UserId, laboratoryWork.Constraints}).ToList()));
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int laboratoryWorkId)
        {
            var laboratoryWorks = await _db.LaboratoryWorks
                .Where(laboratoryWork => laboratoryWork.LaboratoryWorkId == laboratoryWorkId)
                .Select(laboratoryWork => new { laboratoryWork.TemplateLaboratoryWorkId, laboratoryWork.Name, laboratoryWork.Description, laboratoryWork.Evaluation, laboratoryWork.Cipher, laboratoryWork.UserId, laboratoryWork.Constraints})
                .FirstAsync();
            if (laboratoryWorks != null)
            {
                return Ok(laboratoryWorks);
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LaboratoryWorkDTO model)
        {
            var laboratoryWork = 
                new LaboratoryWork { TemplateLaboratoryWorkId = model.TemplateLaboratoryWorkId, Name = model.Name, Description = model.Description, Evaluation = model.Evaluation, Cipher = model.Cipher, UserId = model.UserId, Constraints = model.Constraints};
            try
            {
                await _db.LaboratoryWorks.AddAsync(laboratoryWork);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] LaboratoryWorkDTO model, int laboratoryWorkId)
        {
            var laboratoryWorkUpdate = await _db.LaboratoryWorks.FirstOrDefaultAsync(laboratoryWork => laboratoryWork.LaboratoryWorkId == laboratoryWorkId);
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (laboratoryWorkUpdate != null)
            {
                try
                {
                    if (curUserId != laboratoryWorkUpdate.UserId)
                    {
                        throw new Exception("No access to edit this lab");
                    }
                    laboratoryWorkUpdate.TemplateLaboratoryWorkId = model.TemplateLaboratoryWorkId;
                    laboratoryWorkUpdate.Name = model.Name;
                    laboratoryWorkUpdate.Description = model.Description;
                    laboratoryWorkUpdate.Evaluation = model.Evaluation;
                    laboratoryWorkUpdate.Cipher = model.Cipher;
                    laboratoryWorkUpdate.UserId = model.UserId;
                    laboratoryWorkUpdate.Constraints = model.Constraints;
                    _db.LaboratoryWorks.Update(laboratoryWorkUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int laboratoryWorkId)
        {
            var laboratoryWorkDelete = await _db.LaboratoryWorks.FirstOrDefaultAsync(laboratoryWork => laboratoryWork.LaboratoryWorkId == laboratoryWorkId);
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (laboratoryWorkDelete != null)
            {
                try
                {
                    if (curUserId != laboratoryWorkDelete.UserId)
                    {
                        throw new Exception("No access to delete this lab");
                    }
                    _db.LaboratoryWorks.Remove(laboratoryWorkDelete);
                    await _db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound();
        }
    }
}