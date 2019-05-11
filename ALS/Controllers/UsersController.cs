using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ALS.DTO;
using ALS.EntityСontext;
using ALS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private ApplicationContext _db;

        public UsersController(IConfiguration configuration, IAuthService authService, ApplicationContext db)
        {
            _configuration = configuration;
            _authService = authService;
            _db = db;
        }

        [HttpPost]
        public async Task Login([FromBody] UserLoginDTO model)
        {
            User appUser = _db.Users.FirstOrDefault(u => u.Email == model.Email && _authService.ValidateUserPassword(u.PwHash, model.Password));
            if (appUser != null)
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
        /// <param name="Email">email from request</param>
        /// <param name="appUser">user</param>
        /// <returns></returns>
        private async Task SendIdentityResponse(string Email, User appUser)
        {
            var response = new
            {
                access_token = _authService.GetAuthData(Email, appUser),
                email = appUser.Email
            };

            // сериализация ответа
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpPost]
        public async Task Register([FromBody] UserRegisterDTO model)
        {
            User appUser = new User() { Email = model.Email, Name = model.Name, Surname = model.Surname, Patronymic = model.Patronymic, PwHash = _authService.GetHashedPassword(model.Password) };

            try
            {
                await _db.Users.AddAsync(appUser);
                await _db.SaveChangesAsync();
                await SendIdentityResponse(model.Email, appUser);
            }
            catch (Exception e)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync($"Invalid user data");
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task Test()
        {
            var curUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await Response.WriteAsync("Hello, {curUser}!");
        }

    }
}