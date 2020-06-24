using Generator.MainGen.Structs;

namespace Generator.MainGen.StdGenFunc
{
    public class RndDouble : Rnd
    {
        public RndDouble(bool multypleReturnDatas = false) : base(multypleReturnDatas) { }
        
        public override dynamic Run(FunctionStruct fs)
        {
            var args = fs.ListArgs;
            if (args.Count == 3)
                args.Insert(2, TypesEnum.дробное.ToString());
            if (args.Count == 2)
                args.Add(TypesEnum.дробное.ToString());
            return Next(args, fs.ToString());
        }
    }
}
