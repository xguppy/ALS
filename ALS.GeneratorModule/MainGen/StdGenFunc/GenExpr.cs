using System;
using System.Collections.Generic;
using Generator.MainGen.Structs;

namespace Generator.MainGen.StdGenFunc
{
    public class GenExpr : AFunc
    {
        private ArithmExpr _arithmExpr;
        protected bool _makeFrac = false;
        public GenExpr(Elems e, bool multypleReturnDatas = false)
        {
            _arithmExpr = new ArithmExpr(e);
            MultypleReturnDatas = multypleReturnDatas;
        }

        public override dynamic Run(FunctionStruct fs)
        {
            var args = fs.ListArgs;

            if (args.Count < 1) throw new Exception($"функция #{FuncsEnum.создатьАВ} принимает 1+ параметров ( сложность арифм. выражения | диапазон | тип | зависимость от кол-ва пермененных) | строка = [ {fs.ToString()} ]");
            try
            {
                // получение параметров
                double range = args.Count > 1 ? double.Parse(args[1].Replace('.', ',')) : 25;
                bool isDouble = args.Count > 2 ? args[2].Trim(' ') == TypesEnum.дробное.ToString() : false;
                int countofvars = args.Count > 3 ? int.Parse(args[3]) : 1;
                int finesse = args.Count > 4 ? int.Parse(args[4]) : 2;
                bool frac = args.Count > 5 ? bool.Parse(args[5]) : _makeFrac;
                int diff = int.Parse(args[0]);
                // генерация арифм. выражения
                _arithmExpr.MakeAE(diff, range, isDouble, countofvars, finesse, frac);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка во время парсинга аргументов функции #{FuncsEnum.создатьАВ} | строка = [ {fs.Raw} | ошибка = {ex.Message}]");
            }

            // все возвращаемые значения
            return new List<(string, string)> { 
                ("выражение", _arithmExpr.ExpressionMD),
                ("вычисление", _arithmExpr.Expression),
                ("условие", _arithmExpr.Conditions),
                ("код", _arithmExpr.Code) 
            };
        }
    }
}
