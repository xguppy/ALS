using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class TestRun
    {
        public int TestRunId { get; set; }
        public int SolutionId { get; set; }
        [Required]
        public string[] InputData { get; set; }
        [Required]
        public string[] OutputData { get; set; }
        [Required]
        [Column(TypeName = "jsonb")]
        public string ResultRun { get; set; }
        public Solution Solution;
    }
}