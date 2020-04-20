using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ALS.CheckModule.Compare;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALS.Controllers
{
    public abstract class ComponentController<T>: Controller
    {
        protected static ComponentList<T> ComponentList;
        private readonly ApplicationContext _db;

        protected ComponentController(ApplicationContext db)
        {
            _db = db;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await Task.Run(ComponentList.GetList));

        [HttpGet]
        public FileResult Get([FromQuery] string nameComponent)
        {
            var fileName = $"{nameComponent}.cs";
            var dataBytes = System.IO.File.ReadAllBytes(ComponentList.GetPathToFile(fileName));
            return File(dataBytes, "text/plain", fileName);
        }

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
                await DeleteComponentFromDataBase(nameComponent);
                return Ok("Пользовательский компонент успешно удалён");
            }
            return BadRequest("Не удалось удалить компонент. Возможно его используют другие компоненты");
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile componentFile)
        {
            string sourceCodeChecker;
            using (var sr = new StreamReader(componentFile.OpenReadStream()))
            {
                sourceCodeChecker = await sr.ReadToEndAsync();
            }
            var checkerList = ComponentList.GetList();
            var fileName = componentFile.FileName;
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
                if (newCheckers.Count == 0 || newCheckers.Count > 1 || className != newCheckers[0])
                {
                    noError = false;
                    ModuleGovernor.AllowBuild();
                    await Delete(className);
                }
            }
            ModuleGovernor.AllowBuild();
            if (noError)
            {
                return Ok("Пользовательский компонент успешно добавлен");
            }
            await ComponentList.Delete(componentFile.FileName);
            return BadRequest("Не удалось добавить компонент. Возможно в коде компонента имеются ошибки");
        }
        
        /// <summary>
        /// Удаление компонента из БД
        /// </summary>
        /// <param name="nameComponent"></param>
        public async Task DeleteComponentFromDataBase(string nameComponent)
        {
            var deletingPreparersLab = _db.LaboratoryWorks.ToList().Where(lab => ComponentPredicate(lab, nameComponent));
            foreach (var item in deletingPreparersLab)
            {
                item.Constraints = DeleteComponent(item.Constraints);
                _db.LaboratoryWorks.Update(item);
                await _db.SaveChangesAsync();
            }
        }

        protected abstract bool ComponentPredicate(LaboratoryWork laboratoryWork, string nameComponent);
        protected abstract string DeleteComponent(string constrains);
    }
}