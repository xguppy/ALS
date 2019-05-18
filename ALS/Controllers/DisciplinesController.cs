using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.DTO;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class DisciplinesController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public DisciplinesController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Task.Run(() => _db.Disciplines.Select(d => new { d.Cipher, d.Name}).ToList()));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string cipher)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == cipher);
            if (discipline != null)
            {
                return Ok(await _db.Disciplines.Where(d => d.Cipher == cipher).Select(d => new { d.Cipher, d.Name }).FirstAsync());
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DisciplineDTO model)
        {
            Discipline discipline = new Discipline { Name = model.Name, Cipher = model.Cipher };
            try
            {
                await _db.Disciplines.AddAsync(discipline);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] DisciplineDTO model)
        {
            var disciplineUpdate = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == model.Cipher);

            if (disciplineUpdate != null)
            {
                try
                {
                    disciplineUpdate.Name = model.Name;
                    _db.Disciplines.Update(disciplineUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string cipher)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == cipher);
            if (discipline != null)
            {
                try
                {
                    _db.Disciplines.Remove(discipline);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound();
        }
    }
}