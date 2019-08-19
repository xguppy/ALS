using System;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class AssignedVariantsController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public AssignedVariantsController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] int groupId, [FromHeader] int laboratoryWorkId)
            => Ok(await Task.Run(() => _db.AssignedVariants
                .Where(aw => _db.Variants
                    .Where(var => var.LaboratoryWorkId == laboratoryWorkId).Select(var => var.VariantId)
                    .Contains(aw.VariantId))
                .Where(aw => _db.Users
                    .Where(user => user.GroupId == groupId).Select(user => user.Id)
                    .Contains(aw.UserId)).Select(aw => new {Id = aw.UserId, VariantId = aw.VariantId}).ToList()));

        public async Task<IActionResult> Create([FromBody] AssignedVariantDTO model)
        {
            var assignedVariant = new AssignedVariant {UserId = model.UserId, VariantId = model.VariantId};
            try
            {
                await _db.AssignedVariants.AddAsync(assignedVariant);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                await Response.WriteAsync(e.Message);
            }
            return Ok(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] AssignedVariantDTO model, [FromHeader] int assignedVariantId)
        {
            var assignedVariantUpdate = await _db.AssignedVariants.FirstOrDefaultAsync(aw => aw.AssignedVariantId == assignedVariantId);

            if (assignedVariantUpdate != null)
            {
                try
                {
                    assignedVariantUpdate.UserId = model.UserId;
                    assignedVariantUpdate.VariantId = model.VariantId;
                    _db.AssignedVariants.Update(assignedVariantUpdate);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok(assignedVariantId);
            }
            return NotFound("Update does not exist assigned variant");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] int assignedVariantId)
        {
            var assignedVariant = await _db.AssignedVariants.FirstOrDefaultAsync(aw => aw.AssignedVariantId == assignedVariantId);
            if (assignedVariant != null)
            {
                try
                {
                    _db.AssignedVariants.Remove(assignedVariant);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    await Response.WriteAsync(e.Message);
                }
                return Ok();
            }
            return NotFound("Assigned variant not found");
        }
    }
}