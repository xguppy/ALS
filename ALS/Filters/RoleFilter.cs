using System;
using System.Linq;
using System.Security.Claims;
using ALS.Attributes;
using ALS.EntityСontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ALS.Filters
{
    public class RoleFilter: IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerInfo = (ControllerActionDescriptor)context.ActionDescriptor;

            if (!(Attribute.GetCustomAttribute(controllerInfo.ControllerTypeInfo.AsType(), typeof(RoleAttribute)) is RoleAttribute roleAttr)) return;
            
            var curUser = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            using (var db = new ApplicationContext())
            {
                var check = db
                    .Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefault(usr => usr.Email == curUser)?
                    .UserRoles.FirstOrDefault(roleUser => roleUser.Role.RoleName == roleAttr.RoleName);
                if (check == null)
                {
                    context.Result = new BadRequestResult();
                }
            }
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}