using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ALS.Pages
{
    public class ThemesModel : PageModel
    {
        private ApplicationContext _context;
        public List<Theme> Themes;
        public List<SelectListItem> Disciplines;
        public ThemesModel(ApplicationContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Themes = await Task.Run(() => _context.Themes.Select(x => x).ToList());
            Disciplines = await Task.Run(() => _context.Disciplines.Select(x =>
                     new SelectListItem
                     {
                         Value = x.Cipher,
                         Text = x.Name
                     }).ToList()
                 );
        }
    }
}