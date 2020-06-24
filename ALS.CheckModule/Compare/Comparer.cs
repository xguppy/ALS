using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALS.CheckModule.Compare.Checker;
using ALS.CheckModule.Compare.DataStructures;
using ALS.CheckModule.Compare.Finaliter;
using ALS.CheckModule.Compare.Preparer;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule.Compare
{
    public class Comparer
    {
        private readonly ProcessProgram _model;
        private readonly ProcessProgram _user;
        private static readonly PreparerList PreparerList = new PreparerList();
        private static readonly CheckerList CheckerList = new CheckerList();
        private static readonly FinaliterList FinaliterList = new FinaliterList();
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
            _model = new ProcessProgram(pathModel, _userResult.Input, false);
            _user = new ProcessProgram(pathUser, _userResult.Input, true);
        }
        /// <summary>
        /// Сранение программ
        /// </summary>
        /// <param name="constrains">Ограничения</param>
        public async Task<ResultRun> CompareAsync(Constrains constrains)
        {
            var isStdInput = true;
            await Task.Run(() => PreparerList.Get(constrains.Preparer)?.Prepare(_user.PathToProgram, _model.PathToProgram, _userResult.Input, ref isStdInput));
            _user.IsStdInput = isStdInput;
            _model.IsStdInput = isStdInput;
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
            await Task.Run(() => CheckerList.Get(constrains.Checker).Check(modelOutput, _user.PathToProgram, _model.PathToProgram, ref _userResult));
            await Task.Run(() => FinaliterList.Get(constrains.Finaliter)?.Finalite(_user.PathToProgram, _model.PathToProgram));
            return _userResult;
        }
        /// <summary>
        /// Получение стандартного вывода из программы
        /// </summary>
        private static List<string> GetOutput(ProcessExecute prog)
        {
            var output = new List<string>();

            using var fs = prog.Output;
            while (!fs.EndOfStream)
            {
                var line = fs.ReadLine()?.Split(' ').SelectMany(elem => elem.Split()).Where(elem => elem != String.Empty);
                output.AddRange(line);
            }

            return output;
        }
    }
}