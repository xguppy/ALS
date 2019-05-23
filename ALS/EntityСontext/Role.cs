using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class Role
    {
        public int RoleId { get; set; }
        [Required]
        public RoleEnum RoleName { get; set; }
        
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
