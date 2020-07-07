using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ALS.DTO;
using ALS.EntityСontext;
using ALS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationContext _db;

        public UsersController(IAuthService authService, ApplicationContext db)
        {
            _authService = authService;
            _db = db;
        }

        [HttpPost]
        public async Task Login([FromBody] UserLoginDTO model)
        {
            var appUser = _db.Users.Include(user => user.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefault(u => u.Email == model.Email);
            if (appUser != null && _authService.ValidateUserPassword(appUser.PwHash, model.Password))
            {
                await SendIdentityResponse(model.Email, appUser);
            }
            else
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Неправильный логин или пароль");
            }
        }
        
        /// <summary>
        /// Send response when user successfully register/login
        /// </summary>
        /// <param name="email">email from request</param>
        /// <param name="appUser">user</param>
        /// <returns></returns>
        private async Task SendIdentityResponse(string email, User appUser)
        {
            var response = new
            {
                username = $"{appUser.Name} {appUser.Surname} {appUser.Patronymic}",
                userId = appUser.Id,
                roles = appUser.UserRoles.Select(ur => ur.Role.RoleName.ToString())
            };

            // сериализация ответа
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            HttpContext.Session.SetString("Token", _authService.GetAuthData(email, appUser));
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost]
        public async Task Register([FromBody] UserRegisterDTO model)
        {
            var appUser = new User { Email = model.Email, Name = model.Name, Surname = model.Surname, Patronymic = model.Patronymic, PwHash = _authService.GetHashedPassword(model.Password), GroupId = model.GroupId };
            
            try
            {
                await _db.Users.AddAsync(appUser);
                var roleUsr = await _db.Roles.FirstOrDefaultAsync(role => role.RoleName == model.Role);
                await _db.UserRoles.AddAsync(new UserRole {Role = roleUsr, User = appUser});
                await _db.SaveChangesAsync();
                await SendIdentityResponse(model.Email, appUser);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Неверные данные пользователя");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByGroup([FromHeader] int groupId)
        {
            if (await _db.Groups.Include(group => group.Users).FirstOrDefaultAsync(group => group.GroupId == groupId) !=
                null)
            {
                return Ok(_db.Users.Where(user => user.GroupId == groupId).Select(user => new {user.Email, user.Id, user.Name, user.Surname, user.Patronymic}));
            }

            return BadRequest("Пользователи в группе отсутствуют");
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminUsers()
            => Ok(await Task.Run(() => _db.Users.Where(user => user.UserRoles.Any(ur => ur.Role.RoleName == RoleEnum.Admin)).Select(user => new {user.Email, user.Id, user.Name, user.Surname, user.Patronymic})));
        
        [HttpGet]
        public async Task<IActionResult> GetTeachersUsers()
            => Ok(await Task.Run(() => _db.Users.Where(user => user.UserRoles.Any(ur => ur.Role.RoleName == RoleEnum.Teacher)).Select(user => new {user.Email, user.Id, user.Name, user.Surname, user.Patronymic})));

        [HttpPost]
        public async Task<IActionResult> EditUser([FromHeader] int userId, [FromBody] UserEditDTO model)
        {
            var usr = _db.Users.FirstOrDefault(user => user.Id == userId);

            if (usr == null)
            {
                return BadRequest("Пользователь не найден");
            }

            usr.Email = model.Email;
            usr.Name = model.Name;
            usr.Surname = model.Surname;
            usr.Patronymic = model.Patronymic;

            _db.Users.Update(usr);
            await _db.SaveChangesAsync();
            return Ok(usr);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> ResetPassword([FromHeader] int userId, [FromHeader] string password)
        {
            var usr = await _db.Users.FirstOrDefaultAsync(user => user.Id == userId);
            usr.PwHash = _authService.GetHashedPassword(password);
            _db.Users.Update(usr);
            await _db.SaveChangesAsync();
            return Ok(userId);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromHeader] int userId)
        {
            var usr = await _db.Users.Include(user => user.UserRoles).FirstOrDefaultAsync(user => user.Id == userId);
            try
            {
                _db.Users.Remove(usr);
                await _db.SaveChangesAsync();
                return Ok(userId);
            }
            catch (Exception)
            {
                return BadRequest("Есть зависимые сущности");
            }
        }
    }
}