using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.EntityСontext
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
