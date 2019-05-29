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
using ALS.AntiPlagModule.Services.CompareModels;
using ALS.AntiPlagModule.Services;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class AntiPlagController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly ILexer _lexer;
        private readonly IModelComparable[] _models = { new ModelLCS(), new ModelLevenshtein(), new ModelGST() };

        public AntiPlagController(ApplicationContext db, ILexer lexer)
        {
            _db = db;
            _lexer = lexer;
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

            if (string.IsNullOrEmpty(sourceCode) || settings.CountResults <= 0)
            {
                return BadRequest();
            }

            var responseData = new List<AntiplagiatResponseDTO>();
            var firstToken = _lexer.Parse(sourceCode);

            foreach (var solution in solutions)
            {
                var secondToken = _lexer.Parse(solution.SourceCode);
                var res = new float[_models.Length];

                for (int i = 0; i < res.Length; ++i)
                {
                    res[i] = await Task.Run(() => _models[i].Execute(firstToken, secondToken));
                }

                responseData.Add(new AntiplagiatResponseDTO { SolutionSecondId = solution.SolutionId, AlgorithmsData = res });
            }

            if (settings.SolutionId != null)
            {
                // load to database
                foreach (var data in responseData)
                {
                    // if not in database
                    if (await _db.AntiplagiatDatas.Where(d => d.SolutionFirstId == settings.SolutionId && d.SolutionSecondId == data.SolutionSecondId).FirstOrDefaultAsync() == null)
                    {
                        await _db.AntiplagiatDatas.AddAsync(new AntiplagiatData { SolutionFirstId = settings.SolutionId.Value, SolutionSecondId = data.SolutionSecondId, AlgorithmsData = data.AlgorithmsData });
                        await _db.SaveChangesAsync();
                    }
                }
            }

            return Ok(responseData);
        }
    }

    public class AntiplagiatResponseDTO
    {
        public int SolutionSecondId { get; set; }
        public float[] AlgorithmsData { get; set; }
    }
}