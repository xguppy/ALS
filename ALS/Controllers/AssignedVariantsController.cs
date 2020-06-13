using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ALS.DTO;
using ALS.EntityСontext;
using ALS.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AssignedVariantsController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public AssignedVariantsController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> GetAll([FromHeader] int groupId, [FromHeader] int laboratoryWorkId)
            => Ok(await Task.Run(() => _db.AssignedVariants
                .Where(aw => _db.Variants
                    .Where(var => var.LaboratoryWorkId == laboratoryWorkId).Select(var => var.VariantId)
                    .Contains(aw.VariantId))
                .Where(aw => _db.Users
                    .Where(user => user.GroupId == groupId).Select(user => user.Id)
                    .Contains(aw.UserId)).Select(aw => new {Id = aw.AssignedVariantId,UserId = aw.UserId, VariantId = aw.VariantId}).ToList()));

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> Get([FromHeader] int userId, [FromHeader] int laboratoryWorkId)
            => Ok(await Task.Run(() => _db.AssignedVariants.Where(aw => aw.UserId == userId && aw.Variant.LaboratoryWorkId == laboratoryWorkId)));
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Student")]
        public async Task<IActionResult> GetWorkVariants([FromHeader] string disciplineId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await Task.Run(() => _db.AssignedVariants.Include(av => av.Variant).ThenInclude(var => var.LaboratoryWork).Where(
                    av =>
                        av.UserId == userId && av.Variant.LaboratoryWork.DisciplineCipher == disciplineId)
                .Select(av => new
                {
                    av.AssignedVariantId, av.Variant.LaboratoryWork.Description, av.Variant.LaboratoryWork.Name, av.Variant.VariantId, VarDescription = av.Variant.Description,
                    IsSolved = av.Solutions.Any(sol => sol.IsSolved)
                }).ToList()));
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> Create([FromBody] AssignedVariantDTO model)
        {
            var assignedVariant = new AssignedVariant {UserId = model.UserId, VariantId = model.VariantId, AssignDateTime = DateTime.Now};
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public async Task<IActionResult> RandomAssign([FromHeader] int groupId, [FromHeader] int laboratoryWorkId)
        {
            var variants = _db.Variants.Where(v => v.LaboratoryWorkId == laboratoryWorkId).ToList().Shuffle();
            var students = _db.Users.Where(user => user.GroupId == groupId).ToList();
            if (variants.Count != 0)
            {
                var counter = 0;
                foreach (var student in students)
                {
                    var assignedVariant = await _db.AssignedVariants.FirstOrDefaultAsync(aw => aw.UserId == student.Id && aw.Variant.LaboratoryWorkId == laboratoryWorkId);
                    if (assignedVariant == null)
                    {
                        await _db.AssignedVariants.AddAsync(new AssignedVariant
                            {VariantId = variants[counter].VariantId, UserId = student.Id, AssignDateTime = DateTime.Now});
                    }
                    else
                    {
                        assignedVariant.VariantId = variants[counter].VariantId;
                        _db.AssignedVariants.Update(assignedVariant);
                    }
                    counter = counter + 1 == variants.Count ? 0 : ++counter;
                }
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("No Variants");
        }
    }
}