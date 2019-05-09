using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class Variant
    {
        public int VariantId { get; set; }
        public int LaboratoryWorkId { get; set; }
        public string Description { get; set; }
        public string LinkToModel { get; set; }
        [Column(TypeName = "jsonb")]
        public string InputDataRuns { get; set; }
        public List<Solution> Solutions { get; set; }
        public LaboratoryWork LaboratoryWork { get; set; }
    }
}