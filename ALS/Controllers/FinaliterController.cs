using ALS.CheckModule.Compare.DataStructures;
using ALS.CheckModule.Compare.Finaliter;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ALS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Teacher, Admin")]
    public class FinaliterController : ComponentController<IFinaliter>
    {
        public FinaliterController(ApplicationContext db): base(db)
        {
            ComponentList = new FinaliterList();
        }


        protected override bool ComponentPredicate(LaboratoryWork laboratoryWork, string nameComponent)
            => JsonConvert.DeserializeObject<Constrains>(laboratoryWork.Constraints).Finaliter == nameComponent;

        protected override string DeleteComponent(string constrains)
        {
            var constrainsObject = JsonConvert.DeserializeObject<Constrains>(constrains);
            constrainsObject.Finaliter = null;
            return JsonConvert.SerializeObject(constrainsObject);
        }
    }
}