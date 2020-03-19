using System;
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
        private static readonly CheckerList CheckerList = new CheckerList();
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await Task.Run(CheckerList.GetList));

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string nameChecker) =>
            Ok(await CheckerList.GetText(nameChecker));

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] string nameChecker)
        {
            if (!_isAvailable)
            {
                return BadRequest("В данный момент система производит сборку");
            }
            _isAvailable = false;
            var backUpCheckerCode = await CheckerList.Delete($"{nameChecker}.cs");
            var noError = await CheckerList.ReloadActions();
            _isAvailable = true;
            if(noError)
            {
                return Ok("Чекер успешно удалён");
            }
            await CheckerList.Add(backUpCheckerCode, $"{nameChecker}.cs");
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
            var checkerList = CheckerList.GetList();
            var fileName = checkerFile.FileName;
            var className = fileName.Substring(0, fileName.IndexOf(".cs", StringComparison.Ordinal));
            await CheckerList.Add(sourceCodeChecker, fileName);
            var noError = await CheckerList.ReloadActions();
            var newCheckerList = CheckerList.GetList();
            var newCheckers = newCheckerList.Except(checkerList).ToList();
            //Только один чекер может быть добавлен
            //Имя файла с чекером должно быть аналогично имени класса
            if (newCheckers.Count > 1 || className != newCheckers[0])
            {
                noError = false;
                await Delete(className);
            }
            _isAvailable = true;
            if (noError)
            {
                return Ok("Чекер успешно добавлен");
            }
            await CheckerList.Delete(checkerFile.FileName);
            return BadRequest("Не удалось добавить чекер. Возможно в коде чекере имеются ошибки");
        }
        
    }
}
