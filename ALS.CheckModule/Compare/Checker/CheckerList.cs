using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule.Compare.Checker
{
    public class CheckerList
    {
        private static readonly Dictionary<string, IChecker> Checkers = new Dictionary<string, IChecker>();

        static CheckerList()
        {
            GetCheckers(Checkers);
        }

        public static async Task<bool> ReloadCheckers()
        {
            var result = await BuildCheckModule();
            if(result)
            {
                GetCheckers(Checkers);
            }
            return result;
        }

        private static void GetCheckers(Dictionary<string, IChecker> checkers)
        {
            //Получим сборку 
            var checkModulePath = Path.Combine(GetPathToModule(), "bin", "Release", "netcoreapp2.2", "ALS.CheckModule.dll");
            var checkModuleAssembly = Assembly.LoadFile(checkModulePath);
            //Получим все чекеры
            var checkersAvailable = checkModuleAssembly.GetTypes().Where(t => t.IsClass && typeof(IChecker).IsAssignableFrom(t));
            //Соберем словарь чекеров
            foreach (var checker in checkersAvailable)
            {
                if(!checkers.ContainsKey(checker.Name))
                {
                    checkers.Add(checker.Name, (IChecker)Activator.CreateInstance(checker));
                }
            }
        }

        private static async Task<bool> BuildCheckModule()
        {
            var modulePath = GetPathToModule();
            var dotnetAssembly = new ProcessDotnetReloader(modulePath);
            return await dotnetAssembly.Execute(10000);
        }

        private static string GetPathToModule() => Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "ALS.CheckModule");

        public IChecker this[string nameChecker]
        {
            get
            {
                //Если чекер не задан применим дефолтный
                if (nameChecker == null)
                {
                    nameChecker = "AbsoluteChecker";
                }
                //Если чекера нет в словаре бросим исключение
                if (!Checkers.ContainsKey(nameChecker))
                {
                    throw new Exception("Выбранного чекера не существует");
                }
                return Checkers[nameChecker];
            }
        }

        public static List<string> GetListCheckers()
        {
            var checkList = new List<string>(Checkers.Count);
            foreach (var elem in Checkers)
            {
                checkList.Add(elem.Key);
            }
            return checkList;
        }

        public static async Task AddChecker(string checkerCode, string fileName)
        {
            using (var fileStreamWriter = new StreamWriter(Path.Combine(GetPathToModule(), "Compare", "Checker", fileName)))
            {
                await fileStreamWriter.WriteAsync(checkerCode);
            }
        }

        public static async Task<string> DeleteChecker(string fileName)
        {
            var pathDeleteFile = Path.Combine(GetPathToModule(), "Compare", "Checker", fileName);
            string sourceCode;
            using(var fileStreamReader = new StreamReader(pathDeleteFile))
            {
                sourceCode = await fileStreamReader.ReadToEndAsync();
            }
            File.Delete(pathDeleteFile);
            return sourceCode;
        }
    }
}
