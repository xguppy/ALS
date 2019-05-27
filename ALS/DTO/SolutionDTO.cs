using System;

namespace ALS.DTO
{
    public class SolutionDTO
    {
        public int VariantId { get; set; }
        public DateTime? SendDate { get; set; }
        public string SourceCode { get; set; }
        public int UserId { get; set; }
        public bool IsSolved { get; set; }
    }
}