using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ALS.Pages
{
    public class Variants : PageModel
    {
        public List<LaboratoryWork> LaboratoryWorks { get; set; }
        private readonly ApplicationContext _db;

        public Variants(ApplicationContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            LaboratoryWorks = new List<LaboratoryWork>(_db.LaboratoryWorks.Where(lw => lw.UserId == curUserId));
        }
    }
}