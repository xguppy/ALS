using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule.Compare
{
    public class CompareModel
    {
        private readonly ProcessProgram _model;
        private readonly ProcessProgram _user;
        private List<string> _userOutput;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathModel">Путь до эталонной программы</param>
        /// <param name="pathUser">Путь до пользовательской программы</param>
        /// <param name="inputData">Входные данные</param>
        public CompareModel(string pathModel, string pathUser, List<string> inputData)
        {
            _model = new ProcessProgram(pathModel, inputData);
            _user = new ProcessProgram(pathUser, inputData);
        }
        /// <summary>
        /// Пользовательский вывод
        /// </summary>
        public List<string> UserOutput => _userOutput;
        public async Task<CompareData> Compare(int timeMilliseconds, long memory)
        {
            var okUserProg = false;
            var okModelProg = false;
            //Начнём и подождём завершения
            await Task.Run(() => okModelProg = _model.Execute(timeMilliseconds));
            await Task.Run(() => okUserProg = _user.Execute(timeMilliseconds));
            var userCompare = new CompareData {Time = _user.Time, Memory = _user.Memory, IsCorrect = false};
            if (okUserProg == false || _user.Memory > memory)
            {
                return userCompare;
            }
            if (okModelProg == false)
            {
                throw new Exception("Invalid model");
            }

            //Теперь сравним выводы
            _userOutput = new List<string>();
            using (var fsModel = _model.Output)
            {
                using (var fsUser = _user.Output)
                {
                    while (!fsModel.EndOfStream && !fsUser.EndOfStream)
                    {
                        var userStr = fsUser.ReadLine();
                        _userOutput.Add(userStr);
                        if (fsModel.ReadLine() != userStr)
                        {
                            return userCompare;
                        }
                    }
                    if (!fsModel.EndOfStream)
                    {
                        return userCompare;
                    }
                }
            }
            userCompare.IsCorrect = true;
            return userCompare;
        }
    }
}