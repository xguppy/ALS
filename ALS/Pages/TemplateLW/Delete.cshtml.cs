using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ALS.DTO;
using ALS.EntityСontext;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace ALS.Pages.TemplateLW
{
    public class DeleteModel : PageModel
    {
        public TemplateLaboratoryWork Template = new TemplateLaboratoryWork();
        private ApplicationContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public DeleteModel(ApplicationContext context, IHttpClientFactory fct)
        {
            _context = context;
            _clientFactory = fct;
        }

        public async Task OnGetAsync(int id)
        {
            Template = await Task.Run ( () => _context.TemplateLaboratoryWorks.Include(x => x.Theme).FirstOrDefault(x => x.TemplateLaboratoryWorkId == id));
        }

        public async Task<IActionResult> OnPostDeleting(int tlwId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://localhost:44305/api/TemplateLWS/Delete");

            request.Headers.Add("templateId", tlwId.ToString());

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }
            return RedirectToPage("/Error");
        }
    }
}