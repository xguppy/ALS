using System;

namespace Generator.MainGen.Structs
{
    [Serializable]
    public class ResultData
    {
        public string Template { get; set; }
        public string Code { get; set; }
        public string Tests { get; set; }
        //public  List<Pair<string, string>>  Tests{ get; set; }
    }
}
