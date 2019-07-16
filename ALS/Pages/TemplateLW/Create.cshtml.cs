using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ALS.DTO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ALS.Pages.TemplateLW
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public TemplateLaboratoryWork Template { get; set; } = new TemplateLaboratoryWork();
        private IHostingEnvironment _environment;
        public List<SelectListItem> Themes;
        private ApplicationContext _context;
        public string Message { get; set; } = "123";
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(ApplicationContext ctx, IHostingEnvironment env, IHttpClientFactory fct)
        {
            _context = ctx;
            _environment = env;
            _clientFactory = fct;
        }

        public async Task OnGet()
        {
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
                "https://localhost:44305/api/TemplateLWS/Create");

            request.Content = new StringContent(JsonConvert.SerializeObject(new TemplateLWDTO() { ThemeId = themeId, TemplateTask = new Uri(file).AbsoluteUri}), Encoding.UTF8, "application/json");

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