using System.ComponentModel.DataAnnotations;

namespace ALS.DTO
{
    public class DisciplineDTO
    {
        [Required]
        public string Name { get; set; }
        public string Cipher { get; set; }
    }
}
