using System;
using System.Collections.Generic;

namespace ALS.EntityСontext
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int AssignedVariantId { get; set; }
        public DateTime? SendDate { get; set; }
        public string SourceCode { get; set; }
        public bool IsCompile { get; set; }
        public bool IsSolved { get; set; }
        
        public List<TestRun> TestRuns { get; set; }
        public AssignedVariant AssignedVariant { get; set; }
    }
}