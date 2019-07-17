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
using Microsoft.Extensions.Configuration;
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
            User appUser = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (appUser != null && _authService.ValidateUserPassword(appUser.PwHash, model.Password))
            {
                await SendIdentityResponse(model.Email, appUser);
            }
            else
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid login or password");
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
                access_token = _authService.GetAuthData(email, appUser),
                username = $"{appUser.Name} {appUser.Surname} {appUser.Patronymic}",
                userId = appUser.Id
            };

            // сериализация ответа
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost]
        public async Task Register([FromBody] UserRegisterDTO model)
        {
            User appUser = new User { Email = model.Email, Name = model.Name, Surname = model.Surname, Patronymic = model.Patronymic, PwHash = _authService.GetHashedPassword(model.Password), GroupId = model.GroupId };

            try
            {
                await _db.Users.AddAsync(appUser);
                await _db.SaveChangesAsync();
                await SendIdentityResponse(model.Email, appUser);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid user data");
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task Test()
        {
            var curUser = User.FindFirst(ClaimTypes.Name).Value;
            await Response.WriteAsync($"Hello, {curUser}!");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task TestAdmin()
        {
            await Response.WriteAsync("Done!");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Student")]
        public async Task TestStudent()
        {
            await Response.WriteAsync("Done!");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public async Task TestTeacher()
        {
            await Response.WriteAsync("Done!");
        }

    }
}