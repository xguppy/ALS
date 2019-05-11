using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Password must have more 8 symbols", MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string Patronymic { get; set; }

        [Required]
        public int GroupId { get; set; }
    }
}
