using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ALS.Pages
{
    public class GenExtensionsModel : PageModel
    {
        public ApplicationContext _context;
        public List<GenExtension> Extensions;

        public GenExtensionsModel(ApplicationContext context)
        {
            Extensions = new List<GenExtension>();
            _context = context;
        }

        public async Task OnGet()
        {
            Extensions = await Task.Run(() => _context.GenExtensions.Select(x => x).OrderBy(x => x.GenExtensionId).ToList());
        }
    }
}