using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ALS.CheckModule.Compare.Checker;
using ALS.CheckModule.Compare.DataStructures;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule.Compare
{
    public class Comparer
    {
        private readonly ProcessProgram _model;
        private readonly ProcessProgram _user;
        
        private List<string> _userInput;
        private List<string> _userOutput;
        
        private static Dictionary<string ,IChecker> _checkers = new Dictionary<string ,IChecker>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathModel">Путь до эталонной программы</param>
        /// <param name="pathUser">Путь до пользовательской программы</param>
        /// <param name="inputData">Входные данные</param>
        public Comparer(string pathModel, string pathUser, List<string> inputData)
        {
            _userInput = inputData;
            _model = new ProcessProgram(pathModel, _userInput);
            _user = new ProcessProgram(pathUser, _userInput);
        }
        /// <summary>
        /// Конструктор типа для поиска всех чекеров в проекте
        /// </summary>
        static Comparer()
        {
            //Получим текущую сборку
            var checkModuleAssembly = Assembly.GetExecutingAssembly();
            //Получим все чекеры
            var checkers = checkModuleAssembly.GetTypes().Where(t => t.IsClass && t.Namespace == "ALS.CheckModule.Compare.Checker" && typeof(IChecker).IsAssignableFrom(t));
            //Соберем словарь чекеров
            foreach (var checker in checkers)
            {
                _checkers.Add(checker.Name, (IChecker)Activator.CreateInstance(checker));
            }
        }
        public async Task<ResultRun> CompareAsync(Constrains constrains)
        {
            //Начнём и подождём завершения
            var okUserProg = await _user.Execute(constrains.Time);
            var okModelProg = await _model.Execute(constrains.Time);
            
            var userCompare = new ResultRun { Time = _user.Time, Memory = _user.Memory, IsCorrect = false };
            
            if (okUserProg == false || _user.Memory > constrains.Memory)
            {
                userCompare.Comment = $"Ваше решение использует {_user.Memory / 1024} КБ и {_user.Time} мс времени, требуется {constrains.Memory / 1024} КБ и {constrains.Time} мс";
                return userCompare;
            }
            if (okModelProg == false)
            {
                throw new Exception("Invalid model");
            }
            
            _userOutput = GetOutput(_user);
            var modelOutput = GetOutput(_model);
            //Если чекер не задан применим дефолтный
            if (constrains.Checker == null) constrains.Checker = "AbsoluteChecker";
            //Если чекера нет в словаре бросим исключение
            if(!_checkers.ContainsKey(constrains.Checker)) throw new Exception("There is no such checker");
            userCompare.Output = _userOutput;
            userCompare.Input = _userInput;
            _checkers[constrains.Checker].Check(_userOutput, modelOutput, ref userCompare);
            
            return userCompare;
        }

        private static List<string> GetOutput(ProcessExecute prog)
        {
            var output = new List<string>();
            
            using (var fs = prog.Output)
            {
                while (!fs.EndOfStream)
                {
                    output.Add(fs.ReadLine()?.Trim());
                }
            }

            return output;
        }
    }
}