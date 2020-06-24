using System.Collections.Generic;
using Generator.MainGen.Structs;
using Generator.MainGen.StdGenFunc;

namespace Generator.MainGen
{
    // функции генератора
    public class GenFunctions
    {
        private Dictionary<FuncsEnum, AFunc> _f;
        private const bool UseMultipleReturningParams = true;

        // инициализация функций
        public void Init(Services s)
        {
            _f = new Dictionary<FuncsEnum, AFunc>();
            _f.Add(FuncsEnum.случайное,         new Rnd         (UseMultipleReturningParams));
            _f.Add(FuncsEnum.случайноеЦелое,    new RndInt      (UseMultipleReturningParams));
            _f.Add(FuncsEnum.случайноеДробное,  new RndDouble   (UseMultipleReturningParams));
            _f.Add(FuncsEnum.луа,               new LuaFunc     (UseMultipleReturningParams));
            _f.Add(FuncsEnum.расширениеЛуа,     new LuaExtension(UseMultipleReturningParams));
            _f.Add(FuncsEnum.создатьАВ,         new GenExpr     (s.Elems, UseMultipleReturningParams));
            _f.Add(FuncsEnum.создатьАВдробь,    new GenExprFrac (s.Elems, UseMultipleReturningParams));
        }       

        // вызов необходимых функций
        public List<(string, string)> WhatToDoWithParam(FunctionStruct fs)
        {
            List<(string, string)> ls = new List<(string, string)>();
            
            switch (fs.FuncsEnum)
            {
                // если функция вовсе не фукнция, а просто строка - возвращаем ее
                case FuncsEnum.justString:
                    ls.Add(("0", fs.Raw));
                    break;

                // вызов необходимой функции
                default:
                    ls = _f[fs.FuncsEnum].Call(fs);
                    break;
            }
            return ls;
            /*if (_f == null)
                ls.Add(("0", fs.Raw));
            else
            {
                switch (fs.FuncsEnum)
                {
                    case FuncsEnum.justString:
                        ls.Add(("0", fs.Raw));
                        break;

                    default:
                        ls = _f[fs.FuncsEnum].Call(fs);
                        break;
                }
            }*/
        }
    }
}