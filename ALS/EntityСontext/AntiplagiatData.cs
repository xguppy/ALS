namespace ALS.EntityСontext
{
    public class AntiplagiatData
    {
        public int AntiplagiatDataId { get; set; }
        public int SolutionFirstId { get; set; }
        public int SolutionSecondId { get; set; }
        public Solution SolutionFirst { get; set; }
        public Solution SolutionSecond { get; set; }
    }
}