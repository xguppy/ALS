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
        private int _time;
        private long _memory;

        public int Time => _time;
        public long Memory => _memory;
        
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
                    _memory = _process.PeakWorkingSet64;
                    _time = _process.TotalProcessorTime.Milliseconds;
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