using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class GenExtensionsController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private IWebHostEnvironment _environment;

        public GenExtensionsController(ApplicationContext db, IWebHostEnvironment env)
        {
            _db = db;
            _environment = env;
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
            return Ok(await _db.GenExtensions.Select(ext => new { ext.GenExtensionId, ext.Extension}).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader]int extId)
        {
            var ext = await _db.GenExtensions.FirstOrDefaultAsync(ext => ext.GenExtensionId == extId);
            if (ext != null)
            {
                return Ok(new { ext.GenExtensionId, ext.Extension });
            }
            return NotFound();
        }
        private async Task<IActionResult> UploadDb(string extension)
        {
            extension = extension.Replace('\\', '/');
            if (!System.IO.File.Exists(new Uri(extension).AbsolutePath))
            {
                return NotFound($"Файл {extension} не найден");
            }

            try
            {
                await _db.GenExtensions.AddAsync(new GenExtension { Extension = extension});
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await Response.WriteAsync(ex.InnerException.Message);
                return BadRequest(ex.InnerException.Message);
            }
            return Ok("Успех");
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFileCollection upload)
        {
            if (upload == null)
            {
                return RedirectToPage("");
            }
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users.FirstOrDefault(u => u.Id == userId).Email;
            user = user.Remove(user.LastIndexOf('@'));
            var path = Path.Combine(_environment.ContentRootPath, "genExtensions", user);
            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);
            path = Path.Combine(path, upload[0].FileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await upload[0].CopyToAsync(fileStream);
            }

            return await UploadDb(path);
        }


        [HttpGet]
        public async Task<IActionResult> ReadFile([FromHeader]string path)
        {
            string text;
            using (StreamReader s = new StreamReader(new Uri(path).AbsolutePath))
            {
                text = await s.ReadToEndAsync();
            }
            return Ok(text);
        }

        [HttpPost]
        public async Task<IActionResult> WriteFile([FromBody] Tuple<string, string> data)
        {
            using (StreamWriter sw = new StreamWriter(new Uri(data.Item1).AbsolutePath, false, Encoding.UTF8))
            {
                await sw.WriteLineAsync(data.Item2);
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Tuple<int, string> data)
        {
            var extId = data.Item1;
            var extensionPath = data.Item2;
            if (!System.IO.File.Exists(new Uri(extensionPath).AbsolutePath))
            {
                return NotFound($"Файл {extensionPath} не найден");
            }

            var extension = await _db.GenExtensions.FirstOrDefaultAsync(ext => ext.GenExtensionId == extId);
            if (extension != null)
            {
                try
                {
                    extension.Extension = extensionPath;
                    _db.GenExtensions.Update(extension);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await Response.WriteAsync(ex.InnerException.Message);
                    return BadRequest(ex.InnerException.Message);
                }
                return Ok(extension);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader]int extId)
        {
            var extension = await _db.GenExtensions.FirstOrDefaultAsync(ext => ext.GenExtensionId == extId);
            if (extension != null)
            {
                try
                {
                    _db.GenExtensions.Remove(extension);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    await Response.WriteAsync(ex.InnerException.Message);
                    return BadRequest(ex.InnerException.Message);
                }
                return Ok(extension);
            }
            return NotFound();
        }
    }
}