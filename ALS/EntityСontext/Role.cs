using System.Collections.Generic;

namespace ALS.EntityСontext
{
    public class Role
    {
        public int RoleId { get; set; }
        public RoleEnum RoleName { get; set; }
        
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
