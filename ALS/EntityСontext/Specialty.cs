using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class Specialty
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Group> Groups { get; set; }
    }
}