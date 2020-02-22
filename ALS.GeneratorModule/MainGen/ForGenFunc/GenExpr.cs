using System;
using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Parametr;

namespace Generator.MainGen.ForGenFunc
{
    public class GenExpr : AFunc
    {
        private ArithmExpr _arithmExpr = new ArithmExpr();
        // выражения
        public override string Run(Param param, List<Param> parametrs = null)
        {
            var args = GetArgs(param.RawData, parametrs);

            if (args.Length < 1) throw new Exception($"функция #{FuncsEnum.genAE} принимает 1+ параметров ( сложность арифм. выражения | диапазон | тип | зависимость от кол-ва пермененных) | строка = [ {param} ]");
            try
            {
                double range = args.Length > 1 ? Double.Parse(args[1].Replace('.', ',')) : 100.0;
                bool isDouble = args.Length > 2 ? args[2].Trim(' ') == "double" : true;
                int countofvars = args.Length > 3 ? Int32.Parse(args[3]) : 1;

                _arithmExpr.Run(Int32.Parse(args[0]), range, isDouble, countofvars);
            }
            catch (Exception)
            {
                throw new Exception($"Ошибка во время парсинга аргументов функции #{FuncsEnum.genAE} | строка = [ {param} ]");
            }
            return _arithmExpr.Expression;
        }

        public string ExpressionCodeOnC()
        {
            return _arithmExpr.CodeOnC;
        }
    }
}
