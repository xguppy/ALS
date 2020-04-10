using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ALS.CheckModule.Compare.DataStructures;
using ALS.CheckModule.Compare.Preparer;
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
    public class PreparerController : ComponentController<IPreparer>
    {
        public PreparerController(ApplicationContext db): base(db)
        {
            ComponentList = new PreparerList();
        }


        protected override bool ComponentPredicate(LaboratoryWork laboratoryWork, string nameComponent)
            => JsonConvert.DeserializeObject<Constrains>(laboratoryWork.Constraints).Preparer == nameComponent;

        protected override string DeleteComponent(string constrains)
        {
            var constrainsObject = JsonConvert.DeserializeObject<Constrains>(constrains);
            constrainsObject.Preparer = null;
            return JsonConvert.SerializeObject(constrainsObject);
        }
    }
}