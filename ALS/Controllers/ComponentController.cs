using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ALS.CheckModule.Compare;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALS.Controllers
{
    public abstract class ComponentController<T>: Controller
    {
        protected static ComponentList<T> ComponentList;
        
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await Task.Run(ComponentList.GetList));

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] string nameComponent) =>
            Ok(await ComponentList.GetText(nameComponent));

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] string nameComponent)
        {
            var backUpComponentCode = await ComponentList.Delete($"{nameComponent}.cs");
            var noError = false;
            try
            {
                noError = await ComponentList.ReloadActions();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            finally
            {
                if (!noError)
                {
                    await ComponentList.Add(backUpComponentCode, $"{nameComponent}.cs");
                }
            }
            ModuleGovernor.AllowBuild();
            if(noError)
            {
                return Ok("Пользовательский компонент успешно удалён");
            }
            return BadRequest("Не удалось удалить компонент. Возможно его используют другие компоненты");
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile checkerFile)
        {
            string sourceCodeChecker;
            using (var sr = new StreamReader(checkerFile.OpenReadStream()))
            {
                sourceCodeChecker = await sr.ReadToEndAsync();
            }
            var checkerList = ComponentList.GetList();
            var fileName = checkerFile.FileName;
            var className = fileName.Substring(0, fileName.IndexOf(".cs", StringComparison.Ordinal));
            await ComponentList.Add(sourceCodeChecker, fileName);
            var noError = false;
            try
            {
                noError = await ComponentList.ReloadActions();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            finally
            {
                var newCheckerList = ComponentList.GetList();
                var newCheckers = newCheckerList.Except(checkerList).ToList();
                //Только один чекер может быть добавлен
                //Имя файла с чекером должно быть аналогично имени класса
                if (newCheckers.Count > 1 || className != newCheckers[0])
                {
                    noError = false;
                    await Delete(className);
                }
            }
            ModuleGovernor.AllowBuild();
            if (noError)
            {
                return Ok("Пользовательский компонент успешно добавлен");
            }
            await ComponentList.Delete(checkerFile.FileName);
            return BadRequest("Не удалось добавить компонент. Возможно в коде компонента имеются ошибки");
        }
    }
}