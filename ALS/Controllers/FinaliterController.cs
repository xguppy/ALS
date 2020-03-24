using ALS.CheckModule.Compare.Finaliter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Admin")]
    public class FinaliterController : ComponentController<IFinaliter>
    {
        public FinaliterController()
        {
            ComponentList = new FinaliterList();
        }
    }
}