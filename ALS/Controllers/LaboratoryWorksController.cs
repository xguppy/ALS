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

        // проверка аута
        [HttpGet]
        public IActionResult CheckAuth()
        {
            return Ok(new string("Auth is done!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await Task.Run(() => _db.LaboratoryWorks.Where(lw => lw.UserId == curUserId)
                .Select(laboratoryWork => new { laboratoryWork.LaboratoryWorkId, laboratoryWork.TemplateLaboratoryWorkId, laboratoryWork.ThemeId, laboratoryWork.Name, laboratoryWork.Description, laboratoryWork.Evaluation, laboratoryWork.DisciplineCipher, laboratoryWork.UserId, laboratoryWork.Constraints}).ToList()));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByDiscipline([FromHeader] string disciplineCipher)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;
            if (role == RoleEnum.Teacher.ToString())
            {
                return Ok(await Task.Run(() =>
                    _db.LaboratoryWorks.Where(lw => lw.UserId == userId && lw.Discipline.Cipher == disciplineCipher)
                        .ToList()));
            }
            return Ok(await Task.Run(() =>
                _db.LaboratoryWorks.Where(lw => lw.Discipline.Cipher == disciplineCipher)
                    .ToList()));
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] int laboratoryWorkId)
        {
            var laboratoryWorks = await _db.LaboratoryWorks
                .Where(laboratoryWork => laboratoryWork.LaboratoryWorkId == laboratoryWorkId)
                .Select(laboratoryWork => new { laboratoryWork.TemplateLaboratoryWorkId, laboratoryWork.ThemeId, laboratoryWork.Name, laboratoryWork.Description, laboratoryWork.Evaluation, laboratoryWork.DisciplineCipher, laboratoryWork.UserId, laboratoryWork.Constraints})
                .FirstOrDefaultAsync();
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
                new LaboratoryWork { TemplateLaboratoryWorkId = model.TemplateLaboratoryWorkId, ThemeId = model.ThemeId, Name = model.Name, Description = model.Description, Evaluation = model.Evaluation, DisciplineCipher  = model.DisciplineCipher, UserId = model.UserId, Constraints = model.Constraints};

            if (laboratoryWork.TemplateLaboratoryWorkId != null &&
                laboratoryWork.ThemeId != _db.TemplateLaboratoryWorks.FirstOrDefault(x => x.TemplateLaboratoryWorkId == laboratoryWork.TemplateLaboratoryWorkId).ThemeId)
            {
                return BadRequest("Theme of laboratory work not equal theme TemplateLaboratoryWork");
            }

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
        public async Task<IActionResult> Update([FromBody] LaboratoryWorkDTO model, [FromHeader] int laboratoryWorkId)
        {
            if (model.TemplateLaboratoryWorkId == -1) model.TemplateLaboratoryWorkId = null; // надо что-то сделать с этим
            var laboratoryWorkUpdate = await _db.LaboratoryWorks.FirstOrDefaultAsync(laboratoryWork => laboratoryWork.LaboratoryWorkId == laboratoryWorkId);
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (laboratoryWorkUpdate != null)
            {
                if (model.TemplateLaboratoryWorkId != null &&
                model.ThemeId != _db.TemplateLaboratoryWorks.FirstOrDefault(x => x.TemplateLaboratoryWorkId == model.TemplateLaboratoryWorkId).ThemeId)
                {
                    return BadRequest("Theme of laboratory work not equal theme TemplateLaboratoryWork");
                }

                try
                {
                    if (curUserId != laboratoryWorkUpdate.UserId)
                    {
                        throw new Exception("No access to edit this lab");
                    }
                    laboratoryWorkUpdate.TemplateLaboratoryWorkId = model.TemplateLaboratoryWorkId;
                    laboratoryWorkUpdate.ThemeId = model.ThemeId;
                    laboratoryWorkUpdate.Name = model.Name;
                    laboratoryWorkUpdate.DisciplineCipher = model.DisciplineCipher;
                    laboratoryWorkUpdate.Description = model.Description;
                    laboratoryWorkUpdate.Evaluation = model.Evaluation;
                    laboratoryWorkUpdate.UserId = model.UserId;
                    laboratoryWorkUpdate.Constraints = model.Constraints;
                    _db.LaboratoryWorks.Update(laboratoryWorkUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok(new string("LW successfully updated"));
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] int laboratoryWorkId)
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