using System.Linq;
using System.Threading.Tasks;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher")]
    public class TestRunsController : Controller
    {
        private readonly ApplicationContext _db;
                
        public TestRunsController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader] int solutionId) =>
            Ok(await Task.Run(() => _db.TestRuns.Where(testRun => testRun.SolutionId == solutionId).Select(testRun => new {testRun.InputData, testRun.OutputData, testRun.ResultRun})));
    }
}