using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ALS.Pages.TemplateLW
{
    public class IndexModel : PageModel
    {
        public ApplicationContext Context;
        public List<TemplateLaboratoryWork> Templates;

        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(ApplicationContext context, IHttpClientFactory clientFactory)
        {
            Templates = new List<TemplateLaboratoryWork>();
            Context = context;
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://localhost:44305/api/TemplateLWS/GetAll");

            //request.Headers.Add("Accept", "application/vnd.github.v3+json");
            //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Templates = await response.Content
                    .ReadAsAsync<List<TemplateLaboratoryWork>>();
            }
        }
    }
}