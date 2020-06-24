using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ALS.DTO;
using ALS.EntityСontext;
using Generator.MainGen;
using Generator.MainGen.Structs;
using Generator.Parsing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Student")]
    public class VariantsController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly Gen _gen;
        private IWebHostEnvironment _environment;

        public VariantsController(ApplicationContext db, IWebHostEnvironment env)
        {
            _db = db;
            _gen = new Gen();
            _environment = env;
        }

        public async Task GenNewTask(VariantDTO model, int templateId)
        {
            //ResultData resOfGen = null;
            var path = _db.TemplateLaboratoryWorks
                .FirstOrDefault(twl => twl.TemplateLaboratoryWorkId == templateId).TemplateTask;
            // если условия соблюдены, генерируем данные
            if (path != null)
            {
                var resOfGen = await _gen.Run(new Uri(path).AbsolutePath, model.LaboratoryWorkId, model.VariantNumber, true);

                // перезаписываем введеные пользователем данные
                model.Description = resOfGen.task;
                model.LinkToModel = resOfGen.code;
                model.InputDataRuns = resOfGen.tests;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (await _db.LaboratoryWorks.Where(w =>  w.UserId == userId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Variants
                .Include(variant => variant.LaboratoryWork)
                .Select(v => new {v.VariantId, v.LaboratoryWorkId, v.LaboratoryWork.Name, v.VariantNumber, v.Description, v.LinkToModel, v.InputDataRuns, v.Constraints })
                .ToList()));
            }
            
            return BadRequest("Нет прав для просмотра");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllByLaboratoryId([FromHeader] int laboratoryWorkId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (await _db.LaboratoryWorks.Where(w =>  w.UserId == userId && w.LaboratoryWorkId == laboratoryWorkId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Variants
                    .Include(variant => variant.LaboratoryWork)
                    .Where(v => v.LaboratoryWork.LaboratoryWorkId == laboratoryWorkId)
                    .Select(v => new { v.VariantId, v.LaboratoryWorkId, v.LaboratoryWork.Name, v.VariantNumber, v.Description, v.LinkToModel, v.InputDataRuns, v.Constraints })
                    .OrderBy(x=> x.VariantNumber)
                    .ToList())
                );
            }
            
            return BadRequest("Нет прав для просмотра");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public async Task<IActionResult> Get([FromHeader] int variantId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (await _db.Variants.Include(v => v.LaboratoryWork).Where(v => v.LaboratoryWork.UserId == userId).FirstOrDefaultAsync() != null)
            {
                var variant = await _db.Variants.FirstOrDefaultAsync(v => v.VariantId == variantId);
                if (variant != null)
                {
                    return Ok(await _db.Variants.Where(v => v.VariantId == variantId).Select(v => new {v.VariantId, v.VariantNumber, v.LaboratoryWorkId, v.Description, v.LinkToModel }).FirstAsync());
                }
                return NotFound("Вариант не найден");
            }

            return BadRequest("Нет прав для просмотра");
        }

        public string PathExecute(string filename, int varId, int lwId) => Path.Combine(_environment.ContentRootPath, "executeModel", $"lr{lwId}_var{varId}_{filename}");

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public async Task<IActionResult> UploadSolutionManually(IFormFileCollection upload, [FromHeader] int varId, [FromHeader] int lwId)
        {
            if (upload == null)
            {
                return RedirectToPage("");
            }

            //var file = Path.Combine(_environment.ContentRootPath, "uploads", upload[0].FileName);
            var file = PathExecute(upload[0].FileName, varId, lwId);

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await upload[0].CopyToAsync(fileStream);
            }

            return Ok(file);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public IActionResult IsThereSolution(IFormFileCollection upload, [FromHeader] int varId, [FromHeader] int lwId)
        {
            var result = $"{System.IO.File.Exists(PathExecute(upload[0].FileName, varId, lwId))}";
            return Ok(result);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public async Task<IActionResult> Create([FromBody] VariantDTO model)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var laboratoryWork = await _db.LaboratoryWorks.Where(lw => lw.UserId == userId && lw.LaboratoryWorkId == model.LaboratoryWorkId).FirstOrDefaultAsync();

            if (laboratoryWork != null)
            {
                // если лабораторная работа содержит шаблон
                try
                {
                    // генерируем описание, ссылку на модель, список входных данных
                    if (laboratoryWork.TemplateLaboratoryWorkId != null)
                        await GenNewTask(model, laboratoryWork.TemplateLaboratoryWorkId.Value);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                Variant variant = new Variant {VariantNumber = model.VariantNumber, LaboratoryWorkId = model.LaboratoryWorkId, Description = model.Description, LinkToModel = model.LinkToModel, InputDataRuns = model.InputDataRuns, Constraints = model.Constraints};
                try
                {
                    await _db.Variants.AddAsync(variant);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.InnerException.Message);
                }
                return Ok(model);
            }
            return BadRequest("Нет прав для проведения изменений");
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public async Task<IActionResult> Update([FromBody] VariantDTO model, [FromHeader] int varId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (await _db.Variants.Include(v => v.LaboratoryWork).Where(v => v.LaboratoryWork.UserId == userId && v.LaboratoryWorkId == model.LaboratoryWorkId).FirstOrDefaultAsync() != null)
            {
                var variantUpdate = await _db.Variants.FirstOrDefaultAsync(v => v.VariantId == varId);

                if (variantUpdate != null)
                {
                    // если лабораторная работа содержит шаблон
                    /*try
                    {
                        var laboratoryWork = await _db.LaboratoryWorks.Where(lw => lw.UserId == userId && lw.LaboratoryWorkId == model.LaboratoryWorkId).FirstOrDefaultAsync();
                        // генерируем описание, ссылку на модель, список входных данных
                        if (laboratoryWork.TemplateLaboratoryWorkId != null)
                            await GenNewTask(model, laboratoryWork.TemplateLaboratoryWorkId.Value);
                    }
                    catch (Exception ex)
                    {
                        // пишем ошибку генератора
                        await Response.WriteAsync(ex.Message);
                        // выходим
                        return BadRequest(ex.Message);
                    }*/
                    try
                    {
                        variantUpdate.Description = model.Description;
                        variantUpdate.LaboratoryWorkId = model.LaboratoryWorkId;
                        variantUpdate.InputDataRuns = model.InputDataRuns;
                        variantUpdate.LinkToModel = model.LinkToModel;
                        variantUpdate.VariantNumber = model.VariantNumber;
                        variantUpdate.Constraints = model.Constraints;
                        _db.Variants.Update(variantUpdate);
                        await _db.SaveChangesAsync();
                    }
                    catch (DbUpdateException e)
                    {
                        await Response.WriteAsync(e.Message);
                    }
                    return Ok(varId);
                }
                return NotFound("Вариант не найдент");
            }

            return BadRequest("Нет прав для проведения изменений");
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
        public async Task<IActionResult> Delete([FromHeader] int variantId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (await _db.Variants.Include(v => v.LaboratoryWork).Where(v => v.LaboratoryWork.UserId == userId && v.VariantId == variantId).FirstOrDefaultAsync() != null)
            {
                var variantRemove = await _db.Variants.FirstOrDefaultAsync(v => v.VariantId == variantId);
                if (variantRemove != null)
                {
                    try
                    {
                        _db.Variants.Remove(variantRemove);
                        await _db.SaveChangesAsync();
                    }
                    catch (DbUpdateException e)
                    {
                        await Response.WriteAsync(e.Message);
                    }
                    return Ok();
                }
                return NotFound("Вариант не найден");
            }

            return BadRequest("Нет прав для удаления");
        }
    }
}