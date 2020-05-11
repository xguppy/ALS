using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ALS.Pages
{
    public class ThemesModel : PageModel
    {
        private ApplicationContext _context;
        public List<Theme> Themes;
        public ThemesModel(ApplicationContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Themes = await Task.Run(() => _context.Themes.Select(x => x).ToList());
        }
    }
}