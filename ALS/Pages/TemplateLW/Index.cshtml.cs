using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ALS.Pages.TemplateLW
{
    public class IndexModel : PageModel
    {
        public ApplicationContext _context;
        public List<TemplateLaboratoryWork> Templates;

        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(ApplicationContext context, IHttpClientFactory clientFactory)
        {
            Templates = new List<TemplateLaboratoryWork>();
            _context = context;
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            Templates = await Task.Run( () => _context.TemplateLaboratoryWorks.Include(x => x.Theme).Select(x => x).ToList());
        }
    }
}