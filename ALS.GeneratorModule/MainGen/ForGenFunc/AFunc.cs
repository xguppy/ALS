using Generator.MainGen.Parametr;
using Generator.Parsing;
using System.Linq;
using System.Collections.Generic;

namespace Generator.MainGen.ForGenFunc
{
    public abstract class AFunc
    {
        private IParser _pr = new Parser();

        protected string[] GetArgs(string str, List<Param> parametrs)
        {
            var args = _pr.GetSeparatedArgs(str);
            if (parametrs == null) return args;

            return args.Select(arg => arg.Trim(' ', '\n', '\r')).ToArray();
            /*for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim(' ', '\n', '\r');
                foreach (var elem in parametrs)
                {
                    if (args[i].Contains($"({elem.Name})"))
                    {
                        args[i] = args[i].Replace($"({elem.Name})", elem.Value);
                    }
                }
            }
            return args;*/
        }

        public abstract string Run(Param param, List<Param> parametrs = null);
    }
}
