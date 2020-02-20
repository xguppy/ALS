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

            if (args.Length < 1) throw new Exception($"Func #{FuncsEnum.genAE} take 1+ parametrs ( hardnessOfArithmeticExpression | range | type | countOfVars )");

            double range = args.Length > 1 ? Double.Parse(args[1].Replace('.', ',')) : 100.0;
            bool isDouble = args.Length > 2 ? args[2].Trim(' ') == "double" : true;
            int countofvars = args.Length > 3 ? Int32.Parse(args[3]) : 1;

            _arithmExpr.Run(Int32.Parse(args[0]), range, isDouble, countofvars);
            return _arithmExpr.Expression;
        }

        public string ExpressionCodeOnC()
        {
            return _arithmExpr.CodeOnC;
        }
    }
}
