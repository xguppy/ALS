using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALS.CheckModule.Processes
{
    public class ProcessProgram: ProcessExecute, IExecutable
    {
        /// <summary>
        /// Входны данные для программы
        /// </summary>
        private readonly List<string> _inputData;
        /// <summary>
        /// Конструктор программы 
        /// </summary>
        /// <param name="nameProgram">Имя исполняемого файла</param>
        /// <param name="inputData">Входные данные</param>
        public ProcessProgram(string nameProgram, List<string> inputData)
        {
            AppProcess.StartInfo.FileName = nameProgram;
            _inputData = inputData;
            InitProcess();
        }

        /// <summary>
        /// Время потраченное на выполнение процесса
        /// </summary>
        public int Time { get; private set; }
        //AppProcess.TotalProcessorTime.Milliseconds;
        /// <summary>
        /// Максимальное количество затраченной памяти
        /// </summary>
        public long Memory { get; private set; } 
        //AppProcess.WorkingSet64;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        /// <returns></returns>
        public async Task<bool> Execute(int timeMilliseconds)
        {
            var result = false;
            await Task.Run(() =>
            {
                using (AppProcess)
                {
                    InitExecute();
                    foreach (var elem in _inputData)
                    {
                        Input = elem;
                    }

                    Memory = AppProcess.WorkingSet64;
                    result = AppProcess.WaitForExit(timeMilliseconds);
                    Time = AppProcess.TotalProcessorTime.Milliseconds;
                }
            });
            return result;
        }
    }
}