using System.Collections.Generic;

namespace Generator.Parsing
{
    public class GenData
    {
        public List<DataContainer> Sd { get; set; }
        public List<DataContainer> TestsD { get; set; }
        public string Template { get; set; }
        public string Code { get; set; }

        public GenData(List<DataContainer> sd, string template, string code, List<DataContainer> testsD)
        {
            Sd = sd;
            Template = template;
            Code = code;
            TestsD = testsD;
        }

        public override string ToString()
        {
            string res = $"Template = {Template}   Code = {Code}\nSD:";
            foreach (var s in Sd)
            {
                res += $"\t{s}\n";
            }

            foreach (var s in TestsD)
            {
                res += $"\t{s}\n";
            }
            return res;
        }

    }
}