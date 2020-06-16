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
        private DateTime _startTime; 
        public int Time { get; set; }
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
            _startTime = _process.StartTime;
            try
            {
                while (!_process.HasExited || !_cancellationTokenMeasuring.IsCancellationRequested)
                {

                    Memory = _process.PeakWorkingSet64;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Time = (DateTime.Now - _startTime).Milliseconds;
            }
        }
    }
}