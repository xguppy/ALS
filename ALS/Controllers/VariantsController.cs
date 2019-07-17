using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class VariantsController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public VariantsController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] int labId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (await _db.LaboratoryWorks.Where(w => w.LaboratoryWorkId == labId && w.UserId == userId).FirstOrDefaultAsync() != null)
            {
                return Ok(await Task.Run(() => _db.Variants.Where(v => v.LaboratoryWorkId == labId).Select(v => new {v.VariantId, v.VariantNumber, v.LaboratoryWorkId, v.Description, v.LinkToModel }).ToList()));
            }

            return BadRequest("Not Privilege");
        }

        [HttpGet]
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
                return NotFound("Variant not found");
            }

            return BadRequest("Not Privilege");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VariantDTO model)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (await _db.Variants.Include(v => v.LaboratoryWork).Where(v => v.LaboratoryWork.UserId == userId && v.LaboratoryWorkId == model.LaboratoryWorkId).FirstOrDefaultAsync() != null)
            {
                Variant variant = new Variant {VariantNumber = model.VariantNumber, LaboratoryWorkId = model.LaboratoryWorkId, Description = model.Description, LinkToModel = model.LinkToModel, InputDataRuns = model.InputDataRuns };
                try
                {
                    await _db.Variants.AddAsync(variant);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok(model);
            }
            return BadRequest("Not Privilege");
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] VariantDTO model, [FromHeader] int varId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (await _db.Variants.Include(v => v.LaboratoryWork).Where(v => v.LaboratoryWork.UserId == userId && v.LaboratoryWorkId == model.LaboratoryWorkId).FirstOrDefaultAsync() != null)
            {
                var variantUpdate = await _db.Variants.FirstOrDefaultAsync(v => v.VariantId == varId);

                if (variantUpdate != null)
                {
                    try
                    {
                        variantUpdate.Description = model.Description;
                        variantUpdate.LaboratoryWorkId = model.LaboratoryWorkId;
                        variantUpdate.InputDataRuns = model.InputDataRuns;
                        variantUpdate.LinkToModel = model.LinkToModel;
                        variantUpdate.VariantNumber = model.VariantNumber;
                        _db.Variants.Update(variantUpdate);
                        await _db.SaveChangesAsync();
                    }
                    catch (DbUpdateException e)
                    {
                        await Response.WriteAsync(e.Message);
                    }
                    return Ok(varId);
                }
                return NotFound("Variant not found");
            }

            return BadRequest("Not Privilege");
        }

        [HttpPost]
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
                return NotFound("Variant not found");
            }

            return BadRequest("Not Privilege");
        }
    }
}