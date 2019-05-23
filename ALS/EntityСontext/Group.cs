using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class Group
    {
        public int GroupId { get; set; }
        [Required]
        [StringLength(256, MinimumLength=5)]
        public string Name { get; set; }
        [Required]
        [Range(2019, 2100)]
        public int Year { get; set; }
        public string SpecialityId { get; set; }
        public List<User> Users { get; set; }
        public Specialty Specialty { get; set; }
    }
}