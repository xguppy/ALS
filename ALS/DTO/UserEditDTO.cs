using System.ComponentModel.DataAnnotations;
using ALS.EntityСontext;

namespace ALS.DTO
{
    public class UserEditDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string Patronymic { get; set; }
        
    }
}