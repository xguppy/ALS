using ALS.CheckModule.Compare.Preparer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Admin")]
    public class PreparerController : ComponentController<IPreparer>
    {
        public PreparerController()
        {
            ComponentList = new PreparerList();
        }
    }
}