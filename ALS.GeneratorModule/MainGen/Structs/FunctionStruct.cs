using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.MainGen.Structs
{
    public class FunctionStruct
    {
        public FuncsEnum FuncsEnum { get; set; }
        public string FullFunctionName { get; set; }

        public string Args { get; set; }
        public List<string> ListArgs { get; set; }

        public string Raw { get; set; }


        public FunctionStruct(string name, string raw, string args, List<string> listArgs, FuncsEnum f)
        {
            FullFunctionName = name;
            Raw = raw;
            Args = args;
            FuncsEnum = f;
            ListArgs = listArgs;
        }
    }
}
