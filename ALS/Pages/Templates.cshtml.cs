using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ALS.DTO;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALS.Pages
{
    public class TemplatesModel : PageModel
    {
        public ApplicationContext _context;
        public List<TemplateLaboratoryWork> Templates;
        public TemplateLaboratoryWork NewTemplate;
        public List<SelectListItem> Themes;
        private IHostingEnvironment _environment;
        private IHttpClientFactory _clientFactory;
        public int Id = -1;

        public TemplatesModel(ApplicationContext context, IHttpClientFactory clientFactory, IHostingEnvironment env)
        {
            Templates = new List<TemplateLaboratoryWork>();
            _context = context;
            _clientFactory = clientFactory;
            _environment = env;
            NewTemplate = new TemplateLaboratoryWork();
        }

        public async Task OnGet()
        {
            Templates = await Task.Run(() => _context.TemplateLaboratoryWorks.Include(x => x.Theme).Select(x => x).ToList());
            Themes = await Task.Run(() => _context.Themes.Select(x =>
                     new SelectListItem
                     {
                         Value = x.ThemeId.ToString(),
                         Text = x.Name
                     }).ToList()
                 );
        }

        public async Task<IActionResult> OnPostCreatingAsync(int themeId, IFormFile upload)
        {
            var file = Path.Combine(_environment.ContentRootPath, "uploads", upload.FileName);

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await upload.CopyToAsync(fileStream);
            }

            var request = new HttpRequestMessage(HttpMethod.Post,
                "http://localhost:5000/api/TemplateLWS/Create");

            request.Content = new StringContent(JsonConvert.SerializeObject(new TemplateLWDTO() { ThemeId = themeId, TemplateTask = new Uri(file).AbsoluteUri }), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("");
            }
            return RedirectToPage("/Error");
        }   
        
    }
}