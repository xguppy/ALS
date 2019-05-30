using System;
using System.Collections.Generic;
using Generator.MainGen.Structs;
using Generator.Parsing;
using Newtonsoft.Json;

namespace Generator.MainGen
{
    public class GenFunctions
    { 
        public Random Random = new Random();
        private ArithmExpr _arithmExpr = new ArithmExpr();

        public string[] GetArgs(string str, List<Pair<string, string>> values)
        {
            int pos = str.IndexOf('(') + 1;
            str = str.Substring(pos, str.IndexOf(')') - pos);
            var args = str.Split('|');
            if (values == null) return args;

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim(' ', '\n', '\r');
                foreach (var elem in values)
                {
                    if (args[i].Contains(elem.First))
                    {
                        args[i] = elem.Second;
                    }
                }
            }
            return args;
        }
        // выражения
        public string Expression(string str, List<Pair<string, string>> values)
        {
            var args = GetArgs(str, values);
            _arithmExpr.Run(Int32.Parse(args[0]));
            return _arithmExpr.Expression;
        }

        public string ExpressionCodeOnC()
        {
            return _arithmExpr.CodeOnC;
        }
        //------------------------------------------------

    

        // Рандомизация ----------------------------------
        private string GenValue(string a, string b, string type, Random r, string count = "1")
        {
            string str;
                
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

            return str;
        }

        public string Rnd(string str, List<Pair<string, string>> values = null)
        {
            var args = GetArgs(str, values);

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
        }
        //-----------------------------------------
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
        

        //public async Task<string> GetTestFromJson(string json)
        


        public List<List<string>> GetTestsFromJson(string json)
        {
            List<List<string>> result = new List<List<string>>();

            var tests = JsonConvert.DeserializeObject<List<DataContainer>>(json);

            foreach(var dc in tests)
            {
                if (dc.Data.Count > 1)
                {
                    result.Add(dc.Data);
                }
                else
                {
                    if (dc.Data[0].Contains($"#{FuncsEnum.rnd}"))
                    {
                        result.Add(new List<string>(Rnd(dc.Data[0]).Split(',')));
                    }
                    else
                    {
                        result.Add(dc.Data);
                    }
                }
            }

            return result;
        }

    }
}