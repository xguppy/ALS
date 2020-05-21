using Generator.MainGen.Parametr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.MainGen.StdGenFunc
{
    public class RndInt : Rnd
    {
        public override string Run(Param param)
        {
            var args = GetArgs(param.RawData);
            if (args.Count == 3)
                args.Insert(2, "int");
            if (args.Count == 2)
                args.Add("int");
            return Next(args, param.ToString());
        }
    }
}
