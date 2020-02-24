using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ALS.CheckModule.Compare.Checker;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Admin")]
    public class CheckerController : Controller
    {
        private static bool _isAvailable = true;
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await Task.Run(CheckerList.GetListCheckers));

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string nameChecker) =>
            Ok(await CheckerList.GetTextChecker(nameChecker));

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] string nameChecker)
        {
            if (!_isAvailable)
            {
                return BadRequest("В данный момент система производит сборку");
            }
            _isAvailable = false;
            var backUpCheckerCode = await CheckerList.DeleteChecker($"{nameChecker}.cs");
            var noError = await CheckerList.ReloadCheckers();
            _isAvailable = true;
            if(noError)
            {
                return Ok("Чекер успешно удалён");
            }
            await CheckerList.AddChecker(backUpCheckerCode, $"{nameChecker}.cs");
            return BadRequest("Не удалось удалить чекер. Возможно его используют другие чекеры");
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile checkerFile)
        {
            if (!_isAvailable)
            {
                return BadRequest("В данный момент система производит сборку");
            }
            _isAvailable = false;
            string sourceCodeChecker;
            using (var sr = new StreamReader(checkerFile.OpenReadStream()))
            {
                sourceCodeChecker = await sr.ReadToEndAsync();
            }
            await CheckerList.AddChecker(sourceCodeChecker, checkerFile.FileName);
            var noError = await CheckerList.ReloadCheckers();
            _isAvailable = true;
            if (noError)
            {
                return Ok("Чекер успешно добавлен");
            }
            await CheckerList.DeleteChecker(checkerFile.FileName);
            return BadRequest("Не удалось добавить чекер. Возможно в коде чекере имеются ошибки");
        }
    }
}
