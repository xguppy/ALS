using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule
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
        public CompareModel(string pathModel, string pathUser, string inputData)
        {
            //Берём входные данные
            var inputModel = new Queue<string>();
            using (var fs = new StreamReader(inputData))
            {
                while (!fs.EndOfStream)
                {
                    inputModel.Enqueue(fs.ReadLine());
                }
            }
            //Копируем входные данные для пользователя
            var inputUser = new Queue<string>(inputModel);
            _model = new ProcessProgram(pathModel, inputModel);
            _user = new ProcessProgram(pathUser, inputUser);
            
        }

        public async Task<bool> Compare(int timeMilliseconds)
        {
            bool okUserProg = false, okModelProg = false;
            //Начнём и подождём завершения
            await Task.Run(() => okModelProg = _model.Execute(timeMilliseconds));
            await Task.Run(() => okUserProg = _user.Execute(timeMilliseconds));
            if(okUserProg == false)
                throw new Exception($"Время выполнения программы завышено, требуется {timeMilliseconds} мс" +
                                    $"программа выполнилась за {_model.Time} мс");
            if(okModelProg == false)
                throw new Exception("Неверная модель");
            //Теперь сравним выводы
            using (var fsModel = _model.Output)
            {
                using (var fsUser = _user.Output)
                {
                    while (!fsModel.EndOfStream && !fsUser.EndOfStream)
                    {
                        if (fsModel.ReadLine() != fsUser.ReadLine())
                        {
                            return false;
                        }
                    }
                    
                    if (!fsModel.EndOfStream)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}