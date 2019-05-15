using ALS.EntityСontext;

namespace ALS.Attributes
{
    public class RoleAttribute: System.Attribute
    {
        public RoleEnum RoleName { get; set; }

        public RoleAttribute(RoleEnum roleName)
        {
            RoleName = roleName;
        }
    }
}