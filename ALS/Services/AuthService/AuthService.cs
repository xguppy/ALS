using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ALS.EntityСontext;
using CryptoHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ALS.Services.AuthService
{
    public class AuthService: IAuthService
    {
        // Generates JWT
        private IConfiguration _config { get; }

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GetAuthData(string email, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, $"{user.Surname} {user.Name}")
            };

            using (var db = new ApplicationContext())
            {
                var dbUser = db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefault(u => u.Email == email);
                if (dbUser != null)
                {
                    foreach (var role in dbUser.UserRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName.ToString()));
                    }
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["JwtExpires"]));

            var token = new JwtSecurityToken(
                _config["JwtIssuer"],
                _config["JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetHashedPassword(string pass) => Crypto.HashPassword(pass);

        public bool ValidateUserPassword(string hashed, string pass) => Crypto.VerifyHashedPassword(hashed, pass);
    }
}
