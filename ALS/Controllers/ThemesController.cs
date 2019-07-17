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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class ThemesController : Controller
    {
        private readonly ApplicationContext _db;

        public ThemesController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Themes.Select(t => new { t.Name }).ToListAsync());
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int themeId)
        {
            var theme = await _db.Themes.FirstOrDefaultAsync(t => t.ThemeId == themeId);
            if (theme != null)
            {
                return Ok(new Theme { Name = theme.Name });
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string themeName)
        {
            try
            {
                await _db.Themes.AddAsync(new Theme { Name = themeName });
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
        public async Task<IActionResult> Delete(int themeId)
        {
            var theme = await _db.Themes.FirstOrDefaultAsync(t => t.ThemeId == themeId);
            if (theme != null)
            {
                try
                {
                    _db.Themes.Remove(theme);
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

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] string themeName, int themeId)
        {
            var theme = await _db.Themes.FirstOrDefaultAsync(t => t.ThemeId == themeId);
            if (theme != null)
            {
                try
                {
                    theme.Name = themeName;
                    _db.Themes.Update(theme);
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
