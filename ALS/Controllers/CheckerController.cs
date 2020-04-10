using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ALS.CheckModule.Compare.Checker;
using ALS.CheckModule.Compare.DataStructures;
using ALS.EntityСontext;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Admin")]
    public class CheckerController : ComponentController<IChecker>
    {
        public CheckerController(ApplicationContext db): base(db)
        {
            ComponentList = new CheckerList();
        }


        protected override bool ComponentPredicate(LaboratoryWork laboratoryWork, string nameComponent)
            => JsonConvert.DeserializeObject<Constrains>(laboratoryWork.Constraints).Checker == nameComponent;

        protected override string DeleteComponent(string constrains)
        {
            var constrainsObject = JsonConvert.DeserializeObject<Constrains>(constrains);
            constrainsObject.Checker = null;
            return JsonConvert.SerializeObject(constrainsObject);
        }
    }
}
