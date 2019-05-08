using System;

namespace ALS.CheckModule.Processes
{
    /// <summary>
    /// Процесс компиляции
    /// </summary>
    public class ProcessCompiler: ProcessExecute, IExecutable
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nameInput">Строка исходников</param>
        /// <param name="nameOutput">Выходной файл(сборка)</param>
        /// <param name="arguments">Аргументы компилятора</param>
        /// <param name="pathToCompiler">Путь до компилятора</param>
        public ProcessCompiler(string nameInput, string nameOutput, string arguments = default(string), string pathToCompiler = "g++")
        {
            AppProcess.StartInfo.FileName = pathToCompiler;
            AppProcess.StartInfo.Arguments = $"{arguments} {nameInput} -o {nameOutput}";
            InitProcess();
        }
        /// <summary>
        /// Запуск компиляции
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        public bool Execute(int timeMilliseconds)
        {
            bool result;
            using (AppProcess)
            {
                InitExecute();
                result = AppProcess.WaitForExit(timeMilliseconds);
                var err = Error.ReadToEnd();
                if (err != "")
                {
                    throw new Exception(err);
                }
            }
            return result;

        }
    }
}