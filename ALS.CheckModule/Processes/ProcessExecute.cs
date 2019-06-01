using System.Diagnostics;
using System.IO;

namespace ALS.CheckModule.Processes
{
    public abstract class ProcessExecute
    {
        /// <summary>
        /// Для регистрации процесса
        /// </summary>
        protected readonly Process AppProcess = new Process();
        /// <summary>
        /// Поток ошибки
        /// </summary>
        protected StreamReader ErrorStreamReader;
        /// <summary>
        /// Поток вывода
        /// </summary>
        protected StreamReader OutputStreamReader;
        /// <summary>
        /// Поток ввода
        /// </summary>
        protected StreamWriter InputStreamWriter;
        /// <summary>
        /// Получить строку ошибок
        /// </summary>
        public StreamReader Error => ErrorStreamReader;
        /// <summary>
        /// Получить строку вывода
        /// </summary>
        public StreamReader Output => OutputStreamReader;
        /// <summary>
        /// Ввести строку в поток ввода
        /// </summary>
        protected string Input
        {
            set => InputStreamWriter.WriteLine(value);
        }
        /// <summary>
        /// Время потраченное на выполнение процесса
        /// </summary>
        public int Time => AppProcess.TotalProcessorTime.Milliseconds;
        /// <summary>
        /// Максимальное количество затраченной памяти
        /// </summary>
        public long Memory => AppProcess.WorkingSet64;
        /// <summary>
        /// Инициализация процесса
        /// </summary>
        protected void InitProcess()
        {
            AppProcess.StartInfo.RedirectStandardInput = true;
            AppProcess.StartInfo.RedirectStandardError = true;
            AppProcess.StartInfo.RedirectStandardOutput = true;
            AppProcess.StartInfo.UseShellExecute = false;
        }
        /// <summary>
        /// Инициализация и биндинг потоков
        /// </summary>
        protected void InitExecute()
        {
            AppProcess.Start();
            ErrorStreamReader = AppProcess.StandardError;
            OutputStreamReader = AppProcess.StandardOutput;
            InputStreamWriter = AppProcess.StandardInput;
        }
    }
}