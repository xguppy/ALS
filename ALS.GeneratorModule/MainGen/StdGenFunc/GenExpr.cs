using System;
using Generator.MainGen.Parametr;

namespace Generator.MainGen.StdGenFunc
{
    public class GenExpr : AFunc
    {
        private ArithmExpr _arithmExpr = new ArithmExpr();
                     
        // выражения
        public override string Run(Param param)
        {
            var args = GetArgs(param.RawData);

            if (args.Count < 1) throw new Exception($"функция #{FuncsEnum.genAE} принимает 1+ параметров ( сложность арифм. выражения | диапазон | тип | зависимость от кол-ва пермененных) | строка = [ {param} ]");
            try
            {
                double range = args.Count > 1 ? double.Parse(args[1].Replace('.', ',')) : 100.0;
                bool isDouble = args.Count > 2 ? args[2].Trim(' ') == "double" : true;
                int countofvars = args.Count > 3 ? int.Parse(args[3]) : 1;
                int finesse = args.Count > 4 ? int.Parse(args[4]) : 2;

                _arithmExpr.MakeAE(int.Parse(args[0]), range, isDouble, countofvars, finesse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка во время парсинга аргументов функции #{FuncsEnum.genAE} | строка = [ {param} | ошибка = {ex.Message}]");
            }

            return _arithmExpr.ExpressionMD;
        }

        public string ExpressionCodeOnC()
        {
            return _arithmExpr.Code;
        }
    }
}
