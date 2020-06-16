using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ALS.CheckModule.Compare.Finaliter;

namespace ALS.CheckModule.Processes
{
    public class ProcessProgram: ProcessExecute
    {
        /// <summary>
        /// Входны данные для программы
        /// </summary>
        private readonly List<string> _inputData;
        /// <summary>
        /// Мониторинг ресурсов процесса
        /// </summary>
        private readonly ProcessMonitor _monitor;
        
        private const int TaskDelay = 200;
        /// <summary>
        /// Использовать ли стандартный ввод
        /// </summary>
        public bool IsStdInput { get; set; } = true;
        /// <summary>
        /// Конструктор программы 
        /// </summary>
        /// <param name="nameProgram">Имя исполняемого файла</param>
        /// <param name="inputData">Входные данные</param>
        /// <param name="isMonitoring">Нужно ли запускать монитор для программы</param>
        public ProcessProgram(string nameProgram, List<string> inputData, bool isMonitoring)
        {
            AppProcess.StartInfo.FileName = nameProgram;
            if (isMonitoring)
            {
                _monitor = new ProcessMonitor(AppProcess);
            }
            _inputData = inputData;
            InitProcess();
        }

        /// <summary>
        /// Время потраченное на выполнение процесса
        /// </summary>
        public int Time => _monitor.Time;
        public string PathToProgram => AppProcess.StartInfo.FileName;
        public long Memory => _monitor.Memory;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        /// <returns></returns>
        public override async Task<bool> Execute(int timeMilliseconds)
        {
            var result = false;
            await Task.Run(async () =>
            {
                using (AppProcess)
                {
                    InitExecute();
                    _monitor?.Start();
                    await Task.Delay(TaskDelay);
                    if (IsStdInput)
                    {
                        foreach (var elem in _inputData)
                        {
                            Input = elem;
                        }    
                    }
                    result = AppProcess.WaitForExit(timeMilliseconds);
                    _monitor?.Stop();
                    if (!(_monitor is null))
                    {
                        _monitor.Time = -TaskDelay;
                    }
                }
            });
            return result;
        }
    }
}