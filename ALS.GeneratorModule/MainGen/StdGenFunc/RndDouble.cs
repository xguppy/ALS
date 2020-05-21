using Generator.MainGen.Parametr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.MainGen.StdGenFunc
{
    public class RndDouble : Rnd
    {
        public override string Run(Param param)
        {
            var args = GetArgs(param.RawData);
            if (args.Count == 3)
                args.Insert(2, "double");
            if (args.Count == 2)
                args.Add("double");
            return Next(args, param.ToString());
        }
    }
}
