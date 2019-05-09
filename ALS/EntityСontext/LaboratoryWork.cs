using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class LaboratoryWork
    {
        public int LaboratoryWorkId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Evaluation Evaluation { get; set; }
        public string Cipher { get; set; }
        public string Login { get; set; }
        [Column(TypeName = "jsonb")]
        public string Constraints { get; set; }
        public User User { get; set; }
        public Discipline Discipline { get; set; }
        public List<Variant> Variants { get; set; }
    }
}