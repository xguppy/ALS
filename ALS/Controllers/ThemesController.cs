using System;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // проверка аута
        [HttpGet]
        public IActionResult CheckAuth()
        {
            return Ok(new string("Auth is done!"));
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
        public async Task<IActionResult> Create([FromBody] Tuple<string> themeName)
        {
            try
            {
                await _db.Themes.AddAsync(new Theme { Name = themeName.Item1 });
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await Response.WriteAsync(ex.Message);
                return BadRequest();
            }
            return Ok(themeName);
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader]int themeId)
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
                return Ok(theme);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Tuple<string> themeName, [FromHeader] int themeId)
        {
            var theme = await _db.Themes.FirstOrDefaultAsync(t => t.ThemeId == themeId);
            if (theme != null)
            {
                try
                {
                    theme.Name = themeName.Item1;
                    _db.Themes.Update(theme);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await Response.WriteAsync(ex.Message);
                    return BadRequest();
                }
                return Ok(theme);
            }
            return NotFound();
        }

    }
}
