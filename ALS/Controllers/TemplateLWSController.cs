using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ALS.DTO;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class TemplateLWSController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public TemplateLWSController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.TemplateLaboratoryWorks.Select(twl => new TemplateLWDTO { TemplateTask = twl.TemplateTask, ThemeId = twl.ThemeId}).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader]int templateId)
        {
            var twl = await _db.TemplateLaboratoryWorks.FirstOrDefaultAsync(t => t.TemplateLaboratoryWorkId == templateId);
            if (twl != null)
            {
                return Ok(new TemplateLWDTO { TemplateTask = twl.TemplateTask,ThemeId = twl.ThemeId });
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TemplateLWDTO model)
        {
            if (!System.IO.File.Exists(new Uri(model.TemplateTask).AbsolutePath))
            {
                return NotFound($"File {model.TemplateTask} Not Found");
            }

            try
            {
                await _db.TemplateLaboratoryWorks.AddAsync(new TemplateLaboratoryWork { TemplateTask = model.TemplateTask, ThemeId = model.ThemeId });
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await Response.WriteAsync(ex.Message);
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader]int templateId)
        {
            var template = await _db.TemplateLaboratoryWorks.FirstOrDefaultAsync(twl => twl.TemplateLaboratoryWorkId == templateId);
            if (template != null)
            {
                try
                {
                    _db.TemplateLaboratoryWorks.Remove(template);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await Response.WriteAsync(ex.InnerException.Message);
                    return BadRequest();
                }
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] TemplateLWDTO model, int templateId)
        {
            if (!System.IO.File.Exists(new Uri(model.TemplateTask).AbsolutePath))
            {
                return NotFound($"File {model.TemplateTask} Not Found");
            }

            var template = await _db.TemplateLaboratoryWorks.FirstOrDefaultAsync(twl => twl.TemplateLaboratoryWorkId == templateId);
            if (template != null)
            {
                try
                {
                    template.TemplateTask = model.TemplateTask;
                    template.ThemeId = model.ThemeId;
                    _db.TemplateLaboratoryWorks.Update(template);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await Response.WriteAsync(ex.Message);
                    return BadRequest();
                }
                return Ok();
            }
            return NotFound();
        }

    }
}
