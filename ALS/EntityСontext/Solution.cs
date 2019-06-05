using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int VariantId { get; set; }
        public int CompilerFailsNumbers { get; set; }
        public DateTime? SendDate { get; set; }
        public string SourceCode { get; set; }
        public Variant Variant { get; set; }
        public List<TestRun> TestRuns { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsSolved { get; set; }
    }
}