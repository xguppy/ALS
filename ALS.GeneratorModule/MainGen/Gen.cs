using ALS.CheckModule.Processes;
using Generator.MainGen.Structs;
using Generator.Parsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Generator.MainGen
{
    public class Gen
    {
        // анализ шаблон-файлов
        private Parser _pr = new Parser();
        // доступные функции
        public GenFunctions _gf = new GenFunctions();
        // настройки генератора
        private Services _services = new Services();
        private const char _matchChar = '@';
        private readonly string _fextension;
        public Gen()
        {
            // инициализация стандартных настроек
            InitDefaultServices();
            _gf.Init(_services);
            var os = Environment.OSVersion;
            _fextension = os.Platform == PlatformID.Win32NT ? ".exe" : "";
        }

        private string PathSourceModelCode(string name) => Path.Combine("sourceCodeModel", $"{name}{_fextension}");
        private string PathExecuteModel(string name) => Path.Combine("executeModel", $"{name}{_fextension}");

        private string PathToSoulution(string subpath)
        {
            if (!Directory.Exists(Path.Combine("sourceCodeModel", subpath)))
            {
                Directory.CreateDirectory(Path.Combine("sourceCodeModel", subpath));
            }

            return Path.Combine("sourceCodeModel", subpath);
        }

        private async Task<bool> Compile(int lr, int var)
        {
            string name = ProcessCompiler.CreatePath(lr, var);
            ProcessCompiler pc = new ProcessCompiler(PathToSoulution(name), PathExecuteModel(name));
            return await Task.Run(() => pc.Execute(60000));
        }

        // использование готового объекта-параметра в шаблоне
        private void UseParam(List<(BlockEnum, StringBuilder)> blocks, Param p)
        {
            foreach (var item in blocks)
            {
                // поиск мест где необходима подстановка
                Regex r = new Regex($"({_matchChar})({p.Name})(\\.?)([^{_matchChar}]*)({_matchChar})");
                var ms = r.Matches(item.ToString());
                // непосредственная подстановка параметров
                foreach (Match i in ms)
                {
                    var full = i.Groups[0].ToString();
                    var fieldName = i.Groups[4].ToString();
                    var bestValue = p.GetField(fieldName);
                    item.Item2.Replace(full, bestValue);
                }
            }
        }

        // вывод задания, эталонного решения(кода), тестовых данных (json)
        public (string, string, string) GetTaskCodeTests(List<(BlockEnum, StringBuilder)> blocks)
        {
            var task = blocks.FirstOrDefault(x => x.Item1 == BlockEnum.Template).Item2.ToString();
            var code = blocks.FirstOrDefault(x => x.Item1 == BlockEnum.Solution).Item2.ToString();
            var tests = blocks.FirstOrDefault(x => x.Item1 == BlockEnum.Tests).Item2.ToString();
            return (task, code, JsonConvert.SerializeObject(tests.Trim()));
        }

        // получение очередного параметра из секции ХРАНИЛИЩЕ_ОБЪЕКТОВ
        private IEnumerable<Param> GetNextParam(StringBuilder block)
        {
            // получение объекта-параметра
            while (_pr.GetParamString(block, out string paramStr))
            {
                // создание готового параметра
                var p = CreateParamAsync(paramStr);
                yield return p.Result;
            }
        }

        // создание списка готовых к использования параметров
        private List<Param> GetParams(StringBuilder block)
        {
            List<Param> list = new List<Param>();
            foreach (Param p in GetNextParam(block))
            {
                list.Add(p);
            }
            return list;
        }

        // инициализация стандартных настроек
        private void InitDefaultServices()
        {
            _services.InitDefault();
        }

        // применение конкретных настроек
        private void InitServices(StringBuilder serviceBlock)
        {
            var options = GetParams(serviceBlock);
            _services.Init(options);
            _gf.Init(_services);
        }


        // создание задания, эталонного решения(кода), тестовых данных
        public async Task<(string task, string code, string tests)> GenerateDataAsync(string fileName)
        {
            // получение всех блоков шаблона-файла
            var blocks = await Task.Run(() => _pr.GetBlocks(fileName));
            StringBuilder storage = blocks.FirstOrDefault(x => x.Item1 == BlockEnum.Storage).Item2;
            StringBuilder services = blocks.FirstOrDefault(x => x.Item1 == BlockEnum.Service).Item2;
            InitServices(services);
            foreach (Param p in GetNextParam(storage))
            {
                UseParam(blocks, p);
            }
            return await Task.Run(() => GetTaskCodeTests(blocks));
        }
        // создание готового параметра
        public async Task<Param> CreateParamAsync(string paramStr)
        {
            Param p = await Task.Run(() => _pr.CreateRawParam(paramStr));
            var value = p.GetBestData();
            var fs = await Task.Run(() => _pr.CreateFunctionStruct(value));
            p.SetValue(_gf.WhatToDoWithParam(fs));
            return p;
        }
        
        public async Task<(string task, string code, string tests)> Run(string fileName, int lr = 1, int var = 1, bool needCompile = false)
        {
            (string task, string code,string tests) = await GenerateDataAsync(fileName);
            string name = ProcessCompiler.CreatePath(lr, var);
            string pathtocpp = PathToSoulution(name);

            if (needCompile)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(pathtocpp, $"{name}.cpp"), false, Encoding.UTF8))
                {
                    await sw.WriteLineAsync(code);
                }

                if (!await Compile(lr, var))
                {
                    throw new Exception("Ошибка во время компиляции!");
                }
            }

            return (
                task, 
                new System.Uri(Path.Combine(Environment.CurrentDirectory, PathExecuteModel(name))).AbsoluteUri, 
                tests
            );
        }

        //Создание тестов

        //получить слудующий тест
        private IEnumerable<Test> GetNextTest(StringBuilder block)
        {
            // получение объекта-параметра
            while (_pr.GetParamString(block, out string paramStr))
            {
                // создание готового параметра
                var p = CreateTestAsync(paramStr);
                yield return p.Result;
            }
        }

        // создание списка готовых к использования тестов
        private List<Test> GetTests(StringBuilder block)
        {
            List<Test> list = new List<Test>();
            foreach (Test t in GetNextTest(block))
            {
                list.Add(t);
            }
            return list;
        }

        // установка весов
        private double GetWeigth(string weigth)
        {
            weigth = weigth.Replace('.', ',');
            double res = 20.0;
            if (!double.TryParse(weigth, out res))
            {
                if (int.TryParse(weigth, out int tmp))
                    res = tmp;
            }
            if (res < 1.0e-12)
                res = 20.0;
            return res;
        }

        // создание готового теста
        private async Task<Test> CreateTestAsync(string paramStr)
        {
            Param p = await Task.Run(() => _pr.CreateRawParam(paramStr));
            Test t = new Test();
            t.Weight = GetWeigth(p.Key);
            t.Name = p.Name;
            foreach (var data in p.Data)
            {
                var fs = await Task.Run(() => _pr.CreateFunctionStruct(data.Value));
                var res = _gf.WhatToDoWithParam(fs);
                foreach (var i in res)
                {
                    t.Data.Add(i.Item2);
                }
            }
            return t;
        }

        // получение списка готовых тестовых данных из json
        public List<Test> GetTestsFromJson(string json)
        {
            var t = new StringBuilder(JsonConvert.DeserializeObject<string>(json));
            return GetTests(t);
        }
    }
}