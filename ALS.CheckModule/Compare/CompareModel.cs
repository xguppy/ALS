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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathModel">Путь до эталонной программы</param>
        /// <param name="pathUser">Путь до пользовательской программы</param>
        /// <param name="inputData">Входные данные</param>
        public CompareModel(string pathModel, string pathUser, Queue<string> inputData)
        {
            _model = new ProcessProgram(pathModel, inputData);
            _user = new ProcessProgram(pathUser, inputData);
        }

        public async Task<CompareData> Compare(int timeMilliseconds)
        {
            var okUserProg = false;
            var okModelProg = false;
            //Начнём и подождём завершения
            await Task.Run(() => okModelProg = _model.Execute(timeMilliseconds));
            await Task.Run(() => okUserProg = _user.Execute(timeMilliseconds));
            if(okUserProg == false)
                throw new Exception($"Время выполнения программы завышено, требуется {timeMilliseconds} мс" +
                                    $"программа выполнилась за {_user.Time} мс");
            if(okModelProg == false)
                throw new Exception("Неверная модель");
            var userCompare = new CompareData{Time = _user.Time, Memory = _user.Memory, IsCorrect = false};
            //Теперь сравним выводы
            using (var fsModel = _model.Output)
            {
                using (var fsUser = _user.Output)
                {
                    while (!fsModel.EndOfStream && !fsUser.EndOfStream)
                    {
                        if (fsModel.ReadLine() != fsUser.ReadLine())
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