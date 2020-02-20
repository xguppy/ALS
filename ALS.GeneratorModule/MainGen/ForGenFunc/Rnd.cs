using System;
using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Parametr;

namespace Generator.MainGen.ForGenFunc
{
    public class Rnd : AFunc
    {
        private Random _random = new Random();
        private string _rawstr = string.Empty;
        private string GenValue(string a, string b, string type, Random r, string count = "1")
        {
            string str;
            try
            {
                if (type == "int")
                {
                    Int32 A = Int32.Parse(a), B = Int32.Parse(b);
                    str = RndWrapper.NextIMass(A, B, Int32.Parse(count), r);
                }
                else
                {
                    Double A = Double.Parse(a.Replace('.', ',')), B = Double.Parse(b.Replace('.', ','));
                    str = RndWrapper.NextDMass(A, B, Int32.Parse(count), r);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"\nError in str = {_rawstr}\nFunc #{FuncsEnum.rnd} cannot convert values, maybe it has wrong format | error = {e.Message}");
            }

            return str;
        }

        public override string Run(Param param, List<Param> parametrs = null)
        {
            _rawstr = param.RawData;
            var args = GetArgs(param.RawData, parametrs);

            if (args.Length != 3 && args.Length != 4) throw new Exception($"Func #{FuncsEnum.rnd} take 3(4) parametrs (min | max| type (| count) )");

            string res;

            if (args.Length < 4)
            {
                res = GenValue(args[0], args[1], args[2], _random);
            }
            else
            {
                res = GenValue(args[0], args[1], args[2], _random, args[3]);
            }
            return res;
        }
    }
}
