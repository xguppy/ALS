using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.CheckModule.Processes
{
    public class ProcessMonitor
    {
        private readonly Process _process;
        private readonly Task _monitorTask;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationTokenMeasuring;

        public int Time { get; private set; }
        public long Memory { get; private set; }

        public ProcessMonitor(Process process)
        {
            _process = process;
            _cancellationTokenMeasuring = _tokenSource.Token;
            _monitorTask = new Task(Measuring);
        }

        public void Start()
        {
            _monitorTask.Start();
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void Measuring()
        {
            while (!_process.HasExited || !_cancellationTokenMeasuring.IsCancellationRequested)
            {
                try
                {
                    Memory = _process.PeakWorkingSet64;
                    Time = _process.TotalProcessorTime.Milliseconds;
                }
                catch (Exception)
                {
                    //Если процесс завершён, то обязательно при замерах будет исключение
                    return;
                }
            }
            
        }
    }
}