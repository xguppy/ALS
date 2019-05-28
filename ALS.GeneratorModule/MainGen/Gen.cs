using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Generator.Parsing;
using Generator.MainGen.Structs;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;

namespace Generator.MainGen
{
    public class Gen
    {
        private Parser _pr = new Parser();
        private List<Pair<string, string>> _generated;
        private GenFunctions _genFunctions = new GenFunctions();

        
        //public string Template { get; set; }
        //public string Code { get; set; }
        //public List<DataContainer> Tests { get; set; }
        //public  List<Pair<string, string>>  Tests{ get; set; }
        

        // Выполнение необходимой функции (указаны в файле GenFunctions)
        private string CheckF(string str)
        {
            if (str.Contains(GenFunctions.FuncName.Rnd.Value))
            {
                str = _genFunctions.Rnd(str, _generated);
            }
            else if (str.Contains(GenFunctions.FuncName.GenAE.Value))
            {
                str = _genFunctions.Expression(str, _generated);
            }
            else if (str.Contains(GenFunctions.FuncName.GetAECode.Value))
            {
                str = _genFunctions.ExpressionCodeOnC();
            }
            return str;
        }
        
        private List<Pair<string, string>> ProcessData(List<DataContainer> d)
        {
            List<Pair<string, string>> ls = new List<Pair<string, string>>();
            foreach (var sd in d)
            {
                string res = sd.Data[_genFunctions.Random.Next(0, sd.Data.Count)];
                res = CheckF(res);
                ls.Add(new Pair<string, string>(sd.Name, res));
            }
            return ls;
        }

        
        private string TestsToJSON(List<DataContainer> tests)
        {
            string res = "";
            foreach(var dc in tests)
            {
                string local = $"\"Name\":\"{dc.Name}\",\"Data\":[";
                foreach (var data in dc.Data)
                {
                    local += $"\"{data}\",";
                }
                local = local.TrimEnd(',');
                local += "]";
                local = "{" + local + "},";
                res += local;
            }
            res = res.TrimEnd(',');
            res = "[" + res + "]";
            return res;
        }

        private bool Compile(int lr, int variant)
        {
            ProcessCompiler pc = new ProcessCompiler($"code_lr{lr}_var{variant}.cpp", $"code_lr{lr}_var{variant}.exe");
            return pc.Execute(Int32.MaxValue);
        }

        public async Task<ResultData> Run(string fileName, int lr = 1, int variant = 1)
        {
            // тупа парсинг
            var d = await Task.Run(() => _pr.Read(fileName));
            if (d == null) return null;

            // тупа генерация
            _generated = await Task.Run(() => ProcessData(d.Sd));

            foreach (var elem in _generated)
            {
                var pattern = $"({elem.First})";
                d.Template = d.Template.Replace(pattern, elem.Second);
                d.Code = d.Code.Replace(pattern, elem.Second);
                // кансер шо пипес
                for (int i = 0; i < d.TestsD.Count; i++)
                {
                    for (int j = 0; j < d.TestsD[i].Data.Count; j++)
                    {
                        d.TestsD[i].Data[j] = d.TestsD[i].Data[j].Replace(pattern, elem.Second);
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter($"code_lr{lr}_var{variant}.cpp", false, Encoding.UTF8))
            {
                await sw.WriteLineAsync(d.Code);
            }

            // тупа компиляция
            bool isCompiled = await Task.Run(() => Compile(lr, variant));
            if (!isCompiled)
            {
                throw new Exception("Ошибка во время компиляции!");
            }

            return new ResultData()
            {
                Template = d.Template, /* шаблон задания */
                Code = new System.Uri(Environment.CurrentDirectory + "\\" + $"code_lr{lr}_var{variant}.exe").AbsoluteUri, /* путь до бинарника */
                Tests = TestsToJSON(d.TestsD) /* тестовые данные */
            };
        }
    }
}