using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.EntityСontext
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
