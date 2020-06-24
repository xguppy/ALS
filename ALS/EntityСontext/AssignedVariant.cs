using System;
using System.Collections.Generic;

namespace ALS.EntityСontext
{
    public class AssignedVariant
    {
        public int AssignedVariantId { get; set; }
        public int UserId { get; set; }
        public int VariantId { get; set; }
        public DateTime AssignDateTime { get; set; }
        public double Mark { get; set; }
        
        public User User { get; set; }
        public Variant Variant { get; set; }
        public List<Solution> Solutions { get; set; }
    }
}