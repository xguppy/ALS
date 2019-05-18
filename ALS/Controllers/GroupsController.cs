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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class GroupsController : Controller
    {
        private readonly ApplicationContext _db;

        public GroupsController(ApplicationContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.Run(() => _db.Groups.Select(group => new { group.Name, group.Year, group.SpecialityId}).ToList()));
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int groupId)
        {
            var groups = await _db.Groups
                .Where(group => group.GroupId == groupId)
                .Select(group => new {group.Name, group.Year, group.SpecialityId})
                .FirstAsync();
            if (groups != null)
            {
                return Ok(groups);
            }
            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GroupDTO model)
        {
            var group = new Group { Name = model.Name, Year = model.Year, SpecialityId = model.SpecialityId};
            try
            {
                await _db.Groups.AddAsync(group);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(group);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] GroupDTO model, int groupId)
        {
            var groupUpdate = await _db.Groups.FirstOrDefaultAsync(group => group.GroupId == groupId);

            if (groupUpdate != null)
            {
                try
                {
                    groupUpdate.Name = model.Name;
                    groupUpdate.Year = model.Year;
                    groupUpdate.SpecialityId = model.SpecialityId;
                    _db.Groups.Update(groupUpdate);
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
        public async Task<IActionResult> Delete(int groupId)
        {
            var group = await _db.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group != null)
            {
                try
                {
                    _db.Groups.Remove(group);
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