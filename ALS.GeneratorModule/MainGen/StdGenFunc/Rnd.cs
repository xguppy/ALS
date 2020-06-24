using System;
using System.Collections.Generic;
using Generator.MainGen.Structs;

namespace Generator.MainGen.StdGenFunc
{
    public class Rnd : AFunc
    {
        private Random _random = new Random();

        public Rnd(bool multypleReturnDatas = false)
        {
            MultypleReturnDatas = multypleReturnDatas;
        }

        // генерация случайного значения
        private List<(string, string)> GenValue(string a, string b, string type, Random r, string count = "1")
        {
            List<(string, string)> res;
            if (type == TypesEnum.целое.ToString())
            {
                int A = int.Parse(a), B = int.Parse(b);
                res = RndWrapper.NextIntArray(A, B, int.Parse(count), r);
            }
            else
            {
                double A = double.Parse(a.Replace('.', ',')), B = double.Parse(b.Replace('.', ','));
                res = RndWrapper.NextDoubleArray(A, B, int.Parse(count), r);
            }

            return res;
        }

        // получить следующее случайное значение
        protected List<(string, string)> Next(List<string> args, string rawstr)
        {
            if (args.Count < 3) throw new Exception($"Функция #{FuncsEnum.случайное} принимает 3(4) параметра ( минимум | максимум | тип (| количество) )| строка = [ {rawstr} ]");

            string count = args.Count == 4 ? args[3] : "1";

            try
            {
                return GenValue(args[0], args[1], args[2], _random, count);
            }
            catch (Exception e)
            {
                throw new Exception($"Функция #{FuncsEnum.случайное} не может конвертировать значения, убедитесь что значения имееют формат [double = 0.0] для [int = 0] | строка = [ {rawstr} ] | error = {e.Message} ");
            }
        }

        public override dynamic Run(FunctionStruct fs)
        {
            var args = fs.ListArgs;
            return Next(args, fs.ToString());
        }
    }
}
