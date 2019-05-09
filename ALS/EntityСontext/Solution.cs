using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int VariantId { get; set; }
        public int Mark { get; set; }
        public DateTime SendDate { get; set; }
        [Column(TypeName = "jsonb")]
        public string SourceCode { get; set; }
        public Variant Variant { get; set; }
        public List<TestRun> TestRuns { get; set; }
    }
}