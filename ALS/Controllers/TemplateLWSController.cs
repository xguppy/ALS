using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class TemplateLWSController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public TemplateLWSController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _db.TemplateLaboratoryWorks.Select(twl => new { twl.TemplateTask }).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Get(int templateId)
        {
            var twl = await _db.TemplateLaboratoryWorks.FirstOrDefaultAsync(t => t.TemplateLaboratoryWorkId == templateId);
            if (twl != null)
            {
                return Ok(new { twl.TemplateTask });
            }
            return NotFound();
            //return (twl != null) ? NotFound() : Ok(new { twl.TemplateTask} );
        }


        [HttpPost]
        public async Task<IActionResult> Create(string uriToTemplate)
        {
            if (!System.IO.File.Exists(new Uri(uriToTemplate).AbsolutePath))
            {
                return BadRequest();
            }

            try
            {
                await _db.TemplateLaboratoryWorks.AddAsync(new TemplateLaboratoryWork { TemplateTask = uriToTemplate });
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
        public async Task<IActionResult> Delete(int templateId)
        {
            var template = await _db.TemplateLaboratoryWorks.FirstOrDefaultAsync(twl => twl.TemplateLaboratoryWorkId == templateId);
            if (template != null)
            {
                try
                {
                    _db.TemplateLaboratoryWorks.Remove(template);
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

        [HttpPost]
        public async Task<IActionResult> Update(int templateId, string uriToTemplate)
        {
            var template = await _db.TemplateLaboratoryWorks.FirstOrDefaultAsync(twl => twl.TemplateLaboratoryWorkId == templateId);
            if (template != null)
            {
                try
                {
                    template.TemplateTask = uriToTemplate;
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
