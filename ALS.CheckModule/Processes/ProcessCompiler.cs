using System;
using System.Threading.Tasks;

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
        /// Создание названия файла
        /// </summary>
        /// <param name="lab">Номер лабораторной</param>
        /// <param name="var">Номер варианта</param>
        /// <returns></returns>
        public static string CreatePath(int lab, int var)
            => $"code_lr{lab}_var{var}";
        
        /// <summary>
        /// Информация о компиляции
        /// </summary>
        public string CompileState { get; private set; }
        /// <summary>
        /// Запуск компиляции
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        public async Task<bool> Execute(int timeMilliseconds)
        {
            var result = false;
            await Task.Run(() =>
            {
                using (AppProcess)
                {
                    InitExecute();
                    result = AppProcess.WaitForExit(timeMilliseconds);
                    CompileState = Error.ReadToEnd();
                    if (!String.IsNullOrEmpty(CompileState))
                    {
                        result = false;
                    }
                }
            });
            return result;

        }
    }
}