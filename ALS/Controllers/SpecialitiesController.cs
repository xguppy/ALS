using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ALS.DTO;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ALS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public SpecialitiesController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.Run(() => _db.Specialties.Select(s => new { s.Code, s.Name }).ToList()));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string Code)
        {
            var speciality = await _db.Specialties.FirstOrDefaultAsync(s => s.Code == Code);
            if (speciality != null)
            {
                return Ok(speciality);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecialityDTO model)
        {
            Specialty speciality = new Specialty { Name = model.Name, Code = model.Code };
            try
            {
                await _db.Specialties.AddAsync(speciality);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(speciality);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SpecialityDTO model)
        {
            var specialityUpdate = await _db.Specialties.FirstOrDefaultAsync(s => s.Code == model.Code);

            if (specialityUpdate != null)
            {
                try
                {
                    specialityUpdate.Name = model.Name;
                    _db.Specialties.Update(specialityUpdate);
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
        public async Task<IActionResult> Delete(string Code)
        {
            var speciality = await _db.Specialties.FirstOrDefaultAsync(s => s.Code == Code);
            if (speciality != null)
            {
                try
                {
                    _db.Specialties.Remove(speciality);
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