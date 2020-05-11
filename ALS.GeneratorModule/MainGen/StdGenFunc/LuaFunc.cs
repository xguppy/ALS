using System;
using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Parametr;
using NLua;

namespace Generator.MainGen.StdGenFunc
{
    public class LuaFunc : AFunc
    {
        private Lua _lua;

        public LuaFunc()
        {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
        }

        public override string Run(Param param)
        {
            var args = GetArgs(param.RawData);
            if (args.Length != 1) throw new Exception($"Функция #{FuncsEnum.lua} только 1 параметр| строка = [ {param} ]");
            StringBuilder s = new StringBuilder(args[0]);
            if (s[0] == '\"' && s[s.Length - 1] == '\"')
            {
                s[0] = ' ';
                s[s.Length - 1] = ' ';
            }
            s = s.Replace("io", "Вы не можете использовать stdin/stdout");
            string res = _lua.DoString(s.ToString())[0].ToString();
            return res;
        }
    }
}
