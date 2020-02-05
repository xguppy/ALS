using System;
using System.IO;
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
        /// <param name="nameInputFolderProject">Папка с проектом</param>
        /// <param name="nameOutput">Выходной файл(сборка)</param>
        /// <param name="arguments">Аргументы компилятора</param>
        /// <param name="pathToCompiler">Путь до компилятора</param>
        public ProcessCompiler(string nameInputFolderProject, string nameOutput, string arguments = default, string pathToCompiler = "g++")
        {
            var sourceCodeFiles = Directory.GetFiles(nameInputFolderProject);
            AppProcess.StartInfo.FileName = pathToCompiler;
            AppProcess.StartInfo.Arguments = $"{arguments} {String.Join(' ', sourceCodeFiles)} -o {nameOutput}";
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