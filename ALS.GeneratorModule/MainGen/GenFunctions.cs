using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Generator.MainGen.Parametr;
using Generator.Parsing;
using Newtonsoft.Json;
using System.Text;
using NLua;
using Generator.MainGen.ForGenFunc;

namespace Generator.MainGen
{
    public class GenFunctions
    {
        private List<AFunc> _f = new List<AFunc>(); 

        public GenFunctions()
        {
            _f.Add(new Rnd());
            _f.Add(new GenExpr());
            _f.Add(new LuaFunc());
            _f.Add(new ParentChecker());
        }

        public string WhatToDoWithParam(FuncsEnum funcs, Param param, List<Param> parametrs)
        {
            switch (funcs)
            {
                case FuncsEnum.rnd:
                    return _f[0].Run(param, parametrs);

                case FuncsEnum.genAE:
                    return _f[1].Run(param, parametrs);

                case FuncsEnum.lua:
                    return _f[2].Run(param, parametrs);

                case FuncsEnum.parent:
                    return _f[3].Run(param, parametrs);

                case FuncsEnum.getAEcode:
                    return ((GenExpr)_f[1]).ExpressionCodeOnC();

                default:
                    return param.RawData;

            }

            //return param.RawData;
        }

        public bool CheckTests(List<DataContainer> lDc)
        {
            foreach (DataContainer dc in lDc)
            {
                if (dc.Data.Count == 1) continue;

                foreach (string str in dc.Data)
                {
                    if (str.Contains($"#{FuncsEnum.rnd}"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public List<List<string>> GetTestsFromJson(string json)
        {
            List<List<string>> result = new List<List<string>>();

            var tests = JsonConvert.DeserializeObject<List<DataContainer>>(json);

            foreach (var dc in tests)
            {
                if (dc.Data.Count > 1)
                {
                    result.Add(dc.Data);
                }
                else
                {
                    if (dc.Data[0].Contains($"#{FuncsEnum.rnd}"))
                    {
                        Param p = new Param(dc.Data[0], -1, "TEMP");
                        result.Add(new List<string>(_f[0].Run(p).Split(',')));
                    }
                    else
                    {
                        result.Add(dc.Data);
                    }
                }
            }

            return result;
        }



        /*public Random Random = new Random();
        private ArithmExpr _arithmExpr = new ArithmExpr();
        private IParser _pr = new Parser();
        private Lua _lua;

        public GenFunctions()
        {
            _lua = new Lua();
            _lua.State.Encoding = Encoding.UTF8;
        }

        public string[] GetArgs(string str, List<Param> parametrs)
        {
            var args = _pr.GetSeparatedArgs(str);
            if (parametrs == null) return args;

            for (int i = 0; i < args.Length; i++)
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
            return args;
        }
        // выражения
        public string Expression(string str, List<Param> parametrs)
        {
            var args = GetArgs(str, parametrs);

            if (args.Length != 1) throw new Exception($"Func #{FuncsEnum.genAE} take 1 parametr ( hardnessOfArithmeticExpression )");

            _arithmExpr.Run(Int32.Parse(args[0]));
            return _arithmExpr.Expression;
        }

        public string ExpressionCodeOnC()
        {
            return _arithmExpr.CodeOnC;
        }*/
        //------------------------------------------------


        // Рандомизация ----------------------------------
        /*private string GenValue(string a, string b, string type, Random r, string count = "1")
        {
            string str;
            try
            {
                if (type == "int")
                {
                    Int32 A = Int32.Parse(a), B = Int32.Parse(b);
                    str = RndWrapper.NextIMass(A, B, Int32.Parse(count), r);
                }
                else
                {
                    Double A = Double.Parse(a.Replace('.', ',')), B = Double.Parse(b.Replace('.', ','));
                    str = RndWrapper.NextDMass(A, B, Int32.Parse(count), r);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"func #{FuncsEnum.rnd} cannot convert values, maybe it has wrong format | error = {e}");
            }            

            return str;
        }

        public string Rnd(string str, List<Param> parametrs = null)
        {
            var args = GetArgs(str, parametrs);

            if (args.Length != 3 && args.Length != 4) throw new Exception($"Func #{FuncsEnum.rnd} take 3(4) parametrs (min | max| type (| count) )");

            string res;

            if (args.Length < 4)
            {
                res = GenValue(args[0], args[1], args[2], Random);
            }
            else
            {
                res = GenValue(args[0], args[1], args[2], Random, args[3]);
            }
            return res;
        }*/
        //-----------------------------------------

        //---------------------------------------------------

        /*public string Calculation(string str, List<Param> parametrs = null)
        {
            var args = GetArgs(str, parametrs);
            if (args.Length != 1) throw new Exception($"func #{FuncsEnum.lua} take only 1 arg");
            StringBuilder s = new StringBuilder(args[0]);
            if (s[0] == '\"' && s[s.Length - 1] == '\"')
            {
                s[0] = ' ';
                s[s.Length - 1] = ' ';
            }
            s = s.Replace("io", "you cannot use stdin/out");
            string res = _lua.DoString(s.ToString())[0].ToString();
            //return _engine.Execute(s.ToString()).ToString();
            return res;
        }*/

        //---------------------------------------------------

        

        //--------------------------------------------------------

        /*private List<string> GetParentsFromArg(string arg)
        {
            string[] res;
            if (arg.Contains('&'))
            {
                res = arg.Split('&');
            }
            else
            {
                res = arg.Split(' ');
            }
            return res.Select(x => x.Trim(' ', '\n', '\r')).Where(x => x.Length > 0).ToList();
            //return res.Where(x => x.Length > 0).ToList();
        }

        private List<int> GetPosesFromArg(string arg)
        {
            var poses = arg.Split(' ');
            List<int> res = new List<int>();
            foreach(var s in poses)
            {
                int pos;
                if (!Int32.TryParse(s, out pos)) throw new Exception($"func #{FuncsEnum.parent} posOfParam must be Int32");
                res.Add(pos);
            }
            return res;
        }
        
        public bool FindParent(Param param, List<Param> parametrs)
        {
            string raw = param.RawData;
            var args = GetArgs(param.RawData, parametrs);
            if (args.Length < 2) throw new Exception($"func #{FuncsEnum.parent} takes 2+ parametrs (value | nameOfParent| posOfParam(| posOfParam...))");
            param.RawData = param.Value = args[0];
            StringBuilder code = new StringBuilder($"return {args[1]}");
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
                res = (bool)_lua.DoString(code.ToString())[0];
            }
            catch (Exception ex)
            {
                throw new Exception ($"Error in #parent string = |{raw}|   | message = {ex.Message} |");
            }
            return res;
            var parents = await Task.Run( () => GetParentsFromArg(args[1]));
            var poses   = await Task.Run( () => GetPosesFromArg(args[2]));
            bool IsItAnd = args[1].Contains('&');
            int count = parents.Count;

            if (parents.Count != poses.Count) throw new Exception($"func #{FuncsEnum.parent} count nameOfParent = count posOfParam");
            
            foreach (Param p in parametrs)
            {
                int i = parents.FindIndex(0, x => x == p.Name);
                if (i != -1)
                {
                    bool check = poses[i] != p.Position;
                    if (check && IsItAnd)
                    {
                        return false;
                    }
                    if (!check)
                    {
                        parents.RemoveAt(i);
                        poses.RemoveAt(i);
                    }
                }
            }

            if (parents.Count == 0 || (!IsItAnd && parents.Count < count)) return true;

            return false;
        }*/

    }
}