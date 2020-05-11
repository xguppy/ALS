using System;
using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Parametr;

namespace Generator.MainGen.StdGenFunc
{
    public class Rnd : AFunc
    {
        private Random _random = new Random();
        private string _rawstr = string.Empty;
        private string GenValue(string a, string b, string type, Random r, string count = "1")
        {
            string str;
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

            return str;
        }

        public override string Run(Param param)
        {
            _rawstr = param.RawData;
            var args = GetArgs(param.RawData);

            if (args.Length < 3) throw new Exception($"Функция #{FuncsEnum.rnd} принимает 3(4) параметра ( минимум | максимум | тип (| количество) )| строка = [ {param} ]");

            string res, count = args.Length == 4 ? args[3] : "1";

            try
            {
                res = GenValue(args[0], args[1], args[2], _random, count);
            }
            catch (Exception e)
            {
                throw new Exception($"Функция #{FuncsEnum.rnd} не может конвертировать значения, убедитесь что значения имееют формат [double = 0.0] для [int = 0] | строка = [ {param} ] | error = {e.Message} ");
            }

            return res;
        }
    }
}
