using System;
using System.Collections.Generic;
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

        private static readonly CheckerList CheckerList = new CheckerList();

        private ResultRun _userResult;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathModel">Путь до эталонной программы</param>
        /// <param name="pathUser">Путь до пользовательской программы</param>
        /// <param name="inputData">Входные данные</param>
        public Comparer(string pathModel, string pathUser, List<string> inputData)
        {
            _userResult.Input = inputData;
            _model = new ProcessProgram(pathModel, inputData);
            _user = new ProcessProgram(pathUser, inputData);
        }
        /// <summary>
        /// Сранение программ
        /// </summary>
        /// <param name="constrains">Ограничения</param>
        public async Task<ResultRun> CompareAsync(Constrains constrains)
        {
            //Начнём и подождём завершения
            var okUserProg = await _user.Execute(constrains.Time);
            var okModelProg = await _model.Execute(constrains.Time);
            _userResult.Memory = _user.Memory;
            _userResult.Time = _user.Time;

            if (okUserProg == false || _user.Memory > constrains.Memory)
            {
                _userResult.Comment = $"Ваше решение использует {_user.Memory / 1024} КБ и {_user.Time} мс времени, требуется {constrains.Memory / 1024} КБ и {constrains.Time} мс";
                return _userResult;
            }
            
            if (okModelProg == false)
            {
                throw new Exception("Неверная модель");
            }
            
            _userResult.Output = GetOutput(_user);
            var modelOutput = GetOutput(_model);

            CheckerList[constrains.Checker].Check(modelOutput, _user.PathToProgram, _model.PathToProgram, ref _userResult);
            return _userResult;
        }
        /// <summary>
        /// Получение стандартного вывода из программы
        /// </summary>
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