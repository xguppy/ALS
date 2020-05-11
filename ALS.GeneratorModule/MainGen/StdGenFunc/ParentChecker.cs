using System;
using System.Collections.Generic;
using System.Text;
using Generator.MainGen.Parametr;
using NLua;

namespace Generator.MainGen.StdGenFunc
{
    // DEPRECATED
    public class ParentChecker : AFunc
    {
        private Lua _lua;

        public ParentChecker()
        {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
        }

        public override string Run(Param param)
        {
            /*string raw = param.RawData;
            var args = GetArgs(param.RawData);
            if (args.Length < 2) throw new Exception($"func #{FuncsEnum.parent} takes 2+ parametrs (value | nameOfParent| posOfParam(| posOfParam...)) | строка = [ {param} ]");
            param.RawData = param.Value = args[0];
            StringBuilder str = new StringBuilder(args[1]);
            if (str[0] == str[str.Length - 1] && str[0] == '\"')
            {
                str[0] = ' ';
                str[str.Length-1] = ' ';
            }
            StringBuilder code = new StringBuilder($"return {str.ToString()}");
            if (args.Length > 2)
            {
                for (int i = 2; i < args.Length; i++)
                {
                    var s = args[i].Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (Param p in parametrs)
                    {
                        if (p.Name == s[0] && p.Position == Int32.Parse(s[1]))
                        {
                            code = code.Replace(p.Name, "true");
                        }
                    }
                }
            }
                        
            bool res = false;
            try
            {
                var o = _lua.DoString(code.ToString())[0];
                if (o != null)
                {
                    res = (bool)o;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in #parent string = |{raw}|   | message = {ex.Message} |");
            }*/
            return "ParentChecker is deprecated";
        }
    }
}
