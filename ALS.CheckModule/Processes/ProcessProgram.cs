using System.Collections.Generic;

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
        /// 
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        /// <returns></returns>
        public bool Execute(int timeMilliseconds)
        {
            bool result;
            using (AppProcess)
            {
                InitExecute();
                foreach (var elem in _inputData)
                {
                    Input = elem;
                }
                result = AppProcess.WaitForExit(timeMilliseconds);
            }
            return result;
        }
    }
}