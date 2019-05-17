using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class Discipline
    {
        [Key]
        public string Cipher { get; set; }
        public string Name { get; set; }

        public List<LaboratoryWork> LaboratoryWorks { get; set; }
    }
}