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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationContext _db;
        
        public RoleController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task AddRole([FromHeader] int userId, [FromHeader] RoleEnum role)
        {
            User appUser = await _db.Users.Include(u => u.UserRoles).ThenInclude(r => r.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (appUser != null)
            {
                var curRole = appUser.UserRoles.FirstOrDefault(r => r.Role.RoleName == role);
                if (curRole == null)
                {
                    var roleUsr = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == role);
                    var usrRole = new UserRole{Role = roleUsr, User = appUser};
                    await _db.UserRoles.AddAsync(usrRole);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Role is exist");
                }
            }
            else
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("User not found");
            }
        }
        
        [HttpPost]
        public async Task DeleteRole([FromHeader] int userId, [FromHeader] RoleEnum role)
        {
            User appUser = await _db.Users.Include(u => u.UserRoles).ThenInclude(r => r.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (appUser != null)
            {
                var curRole = appUser.UserRoles.FirstOrDefault(r => r.Role.RoleName == role);
                if (curRole != null)
                {
                    _db.UserRoles.Remove(curRole);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Role not found");
                }
            }
            else
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("User not found");
            }
        }
    }
}