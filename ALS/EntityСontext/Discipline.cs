using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class Discipline
    {
        [Key]
        public string Cipher { get; set; }
        [Required]
        [StringLength(256, MinimumLength=5)]
        public string Name { get; set; }

        public List<LaboratoryWork> LaboratoryWorks { get; set; }
        public List<Plan> Plans { get; set; }
    }
}