using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Generator.Parsing;
using Generator.MainGen.Structs;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;
using Newtonsoft.Json;
using Generator.MainGen;

namespace Generator.MainGen
{
    public class Gen
    {
        private Parser _pr = new Parser();
        private List<Pair<string, string>> _generated;
        private GenFunctions _genFunctions = new GenFunctions();

        // Выполнение необходимой функции (указаны в файле GenFunctions)
        private string CheckF(string str)
        {
            if (str.Contains($"#{FuncsEnum.rnd}"))
            {
                str = _genFunctions.Rnd(str, _generated);
            }
            else if (str.Contains($"#{FuncsEnum.genAE}"))
            {
                str = _genFunctions.Expression(str, _generated);
            }
            else if (str.Contains($"#{FuncsEnum.getAEcode}"))
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

        private async Task<bool> Compile(int lr,int var)
        {
            string lrPath = ProcessCompiler.CreatePath(lr, var);
            ProcessCompiler pc = new ProcessCompiler($"sourceCodeModel\\{lrPath}.cpp", $"executeModel\\{lrPath}.exe");
            return await Task.Run (() => pc.Execute(60000));
        }

        public async Task<ResultData> Run(string fileName,int lr = 1, int var = 1)
        {     
            // тупа парсинг
            var d = await Task.Run( () => _pr.Read(fileName));
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

            if (!_genFunctions.CheckTests(d.TestsD))
            {
                throw new Exception("Тестовые данные содержат ошибку!");
            }

            string lrPath = ProcessCompiler.CreatePath(lr, var);

            using (StreamWriter sw = new StreamWriter($"sourceCodeModel\\{lrPath}.cpp", false, Encoding.UTF8))
            {
                await sw.WriteLineAsync(d.Code);
            }

            // тупа компиляция
            if (!await Compile(lr, var))
            {
                throw new Exception("Ошибка во время компиляции!");
            }        
            

            return new ResultData() {
                Template = d.Template, /* шаблон задания */
                Code = new System.Uri(Environment.CurrentDirectory + $"\\executeModel\\{lrPath}.exe").AbsoluteUri, /* путь до бинарника */
                Tests = JsonConvert.SerializeObject(d.TestsD) /* тестовые данные */
            };
        }
    }
}