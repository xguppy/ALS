using System;
using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Parametr;
using NLua;

namespace Generator.MainGen.ForGenFunc
{
    public class LuaFunc : AFunc
    {
        private Lua _lua;

        public LuaFunc()
        {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
        }

        public override string Run(Param param, List<Param> parametrs = null)
        {
            var args = GetArgs(param.RawData, parametrs);
            if (args.Length != 1) throw new Exception($"func #{FuncsEnum.lua} take only 1 arg");
            StringBuilder s = new StringBuilder(args[0]);
            if (s[0] == '\"' && s[s.Length - 1] == '\"')
            {
                s[0] = ' ';
                s[s.Length - 1] = ' ';
            }
            s = s.Replace("io", "you cannot use stdin/out");
            string res = _lua.DoString(s.ToString())[0].ToString();
            return res;
        }
    }
}
