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
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.Run(() => _db.Disciplines.Select(d => new { d.Cipher, d.Name}).ToList()));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string Cipher)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == Cipher);
            if (discipline != null)
            {
                return Ok(discipline);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task Create([FromBody] DisciplineDTO model)
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
        }

        [HttpPost]
        public async Task Update([FromBody] DisciplineDTO model)
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
            }
            else
            {
                await Response.WriteAsync("Discipline not found");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Cipher)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.Cipher == Cipher);
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
            else
            {
                return NotFound();
            }
        }
    }
}