using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.DTO
{
    public class VariantDTO
    {
        public int VariantId { get; set; }
        public int LaboratoryWorkId { get; set; }
        public string Description { get; set; }
        public string LinkToModel { get; set; }
        public string InputDataRuns { get; set; }
    }
}
