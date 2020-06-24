using Generator.MainGen.Structs;

namespace Generator.MainGen.StdGenFunc
{
    public class GenExprFrac : GenExpr
    {
        public GenExprFrac(Elems e, bool multypleReturnDatas = false) : base(e, multypleReturnDatas)
        {
            _makeFrac = true;
        }

        public override dynamic Run(FunctionStruct fs)
        {
            return base.Run(fs);
        }

    }
}
