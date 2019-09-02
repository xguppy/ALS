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
    [Authorize]
    public class DisciplinesController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public DisciplinesController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher, Student")]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;
            if (role == RoleEnum.Teacher.ToString())
            {
                return Ok(await Task.Run(() =>
                    _db.Plans.Where(d => d.UserId == userId).Select(d => new {d.Discipline.Cipher, d.Discipline.Name}).Distinct().ToList()));
            }
            if (role == RoleEnum.Student.ToString())
            {
                var groupId =  _db.Users.Where(user => user.Id == userId).Select(user => user.GroupId)
                    .FirstOrDefaultAsync().Result;
                if (groupId != null)
                {
                    return Ok(await Task.Run(() =>
                        _db.Plans.Where(d => d.GroupId == groupId).Select(d => new {d.Discipline.Cipher, d.Discipline.Name}).Distinct().ToList()));
                }
                return BadRequest("The student is not in the group");
            }
            return Ok(await Task.Run(() => _db.Disciplines.ToList()));
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> Get([FromHeader] string cipher)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == cipher);
            if (discipline != null)
            {
                return Ok(await _db.Disciplines.Where(d => d.Cipher == cipher).Select(d => new { d.Cipher, d.Name }).FirstAsync());
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> Create([FromBody] DisciplineDTO model)
        {
            Discipline discipline = new Discipline { Name = model.Name, Cipher = model.Cipher };
            try
            {
                await _db.Disciplines.AddAsync(discipline);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> Update([FromBody] DisciplineDTO model)
        {
            var disciplineUpdate = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == model.Cipher);

            if (disciplineUpdate != null)
            {
                try
                {
                    disciplineUpdate.Name = model.Name;
                    _db.Disciplines.Update(disciplineUpdate);
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> Delete([FromHeader] string cipher)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == cipher);
            if (discipline != null)
            {
                try
                {
                    _db.Disciplines.Remove(discipline);
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