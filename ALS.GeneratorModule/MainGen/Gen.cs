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
using Generator.MainGen.Parametr;

namespace Generator.MainGen
{
    public class Gen
    {
        private IParser _pr;
        private IParamsContainer _paramsContainer;
        private List<Param> _parametrs;
        private GenFunctions _genFunctions = new GenFunctions();

        public Gen(IParser pr, IParamsContainer paramsContainer)
        {
            _pr = pr;
            _paramsContainer = paramsContainer;
        }

        private string PathSourceModelCode(string name, string fextension) => Path.Combine("sourceCodeModel", $"{name}.{fextension}");
        private string PathExecuteModel(string name, string fextension) => Path.Combine("executeModel", $"{name}.{fextension}");

        private string PathToSoulution(string subpath)
        {
            if (!Directory.Exists($"sourceCodeModel\\{subpath}"))
            {
                Directory.CreateDirectory($"sourceCodeModel\\{subpath}");
            }

            return $"sourceCodeModel\\{subpath}";
        }

        private async Task<bool> Compile(int lr, int var)
        {
            string name = ProcessCompiler.CreatePath(lr, var);
            //ProcessCompiler pc = new ProcessCompiler(Path.Combine("sourceCodeModel", $"{lrPath}.cpp"), Path.Combine("executeModel", $"{lrPath}.exe"));
            ProcessCompiler pc = new ProcessCompiler(PathToSoulution(name), PathExecuteModel(name, "exe"));
            return await Task.Run(() => pc.Execute(60000));
        }

        public async Task<ResultData> Run(string fileName, int lr = 1, int var = 1, bool needCompile = false)
        {
            var d = await Task.Run(() => _pr.Read(fileName));
            if (d == null) return null;

            _parametrs = _paramsContainer.GenNewParametrs(d.Sd);

            foreach (var elem in _parametrs)
            {
                var pattern = $"@{elem.Name}@";
                d.Template = d.Template.Replace(pattern, elem.Value);
                d.Code = d.Code.Replace(pattern, elem.Value);
                if (d.TestsD == null) continue;
                for (int i = 0; i < d.TestsD.Count; i++)
                {
                    for (int j = 0; j < d.TestsD[i].Data.Count; j++)
                    {
                        d.TestsD[i].Data[j] = d.TestsD[i].Data[j].Replace(pattern, elem.Value);
                    }
                }
            }

            if (d.TestsD != null && !_genFunctions.CheckTests(d.TestsD))
            {
                throw new Exception("Тестовые данные содержат ошибку!");
            }

            string name = ProcessCompiler.CreatePath(lr, var);
            string pathtocpp = PathToSoulution(name);

            if (needCompile)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(pathtocpp, $"{name}.cpp"), false, Encoding.UTF8))
                {
                    await sw.WriteLineAsync(d.Code);
                }

                if (!await Compile(lr, var))
                {
                    throw new Exception("Ошибка во время компиляции!");
                }
            }

            return new ResultData()
            {
                Template = d.Template, /* шаблон задания */
                Code = new System.Uri(Path.Combine(Environment.CurrentDirectory, PathExecuteModel(name, "exe"))).AbsoluteUri, /*путь до бинарника*/
                Tests = JsonConvert.SerializeObject(d.TestsD) /* тестовые данные */
            };
        }
    }
}