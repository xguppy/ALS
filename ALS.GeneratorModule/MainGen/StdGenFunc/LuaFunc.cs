using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Structs;
using NLua;

namespace Generator.MainGen.StdGenFunc
{
    // исполнения функции написанной на языке Lua
    public class LuaFunc : AFunc
    {
        private Lua _lua;

        public LuaFunc(bool multypleReturnDatas = false)
        {
            MultypleReturnDatas = multypleReturnDatas;
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
        }

        public override dynamic Run(FunctionStruct fs)
        {
            // получение кода на lua из аргумента
            StringBuilder s = new StringBuilder(fs.Args);
            if (s[0] == '\"' && s[^1] == '\"')
            {
                s[0] = ' ';
                s[^1] = ' ';
            }
            // исполнение кода
            var res = _lua.DoString(s.ToString());
            // вовзращение одного значения, таблицы или списка значений
            if (res.Length == 1 && res[0].ToString() == "table") 
                return res[0];

            List<(string, string)> ls = new List<(string, string)>();
            for (int i = 0; i < res.Length; i++)
            {
                ls.Add((i.ToString(), res[i].ToString()));
            }
            return ls;
        }
    }
}
