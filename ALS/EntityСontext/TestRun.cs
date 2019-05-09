using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class TestRun
    {
        public int TestRunId { get; set; }
        public int SolutionId { get; set; }
        public string[] InputData { get; set; }
        public string[] OutputData { get; set; }
        [Column(TypeName = "jsonb")]
        public string ResultRun { get; set; }
        public Solution Solution;
    }
}