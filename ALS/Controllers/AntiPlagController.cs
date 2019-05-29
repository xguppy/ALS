using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ALS.DTO;
using Microsoft.EntityFrameworkCore;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class AntiPlagController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public AntiPlagController(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Check([FromBody] AntiplagSettingsDTO settings)
        {
            // get values from database
            var solutions = _db.Solutions.Where(s => s.IsSolved && s.SolutionId != settings.SolutionId);
            string solutionCode = default(string);

            if (!settings.CheckUserWork && settings.SolutionId != null) 
            {
                var solution = await _db.Solutions.Where(s => s.SolutionId == settings.SolutionId).FirstOrDefaultAsync();
                solutions = solutions.Where(s => s.UserId != solution.UserId);
                solutionCode = solution.SourceCode;
            }
            if (settings.CheckTime)
            {
                if (settings.DateTimeFirst != null && settings.DateTimeLast != null && settings.DateTimeFirst < settings.DateTimeLast)
                {
                    solutions = solutions.Where(s => settings.DateTimeFirst < s.SendDate && s.SendDate < settings.DateTimeLast);
                }
                else
                {
                    return BadRequest();
                }
            }

            var sourceCode = settings.SolutionId == null ? settings.SourceCode : solutionCode;

            if (string.IsNullOrEmpty(sourceCode))
            {
                return BadRequest();
            }
            
            // TODO: cycle by works

            return Ok();
        }
    }
}