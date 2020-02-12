using System;
using System.Threading.Tasks;

namespace ALS.CheckModule.Processes
{
    class ProcessDotnetReloader: ProcessExecute
    {
        public ProcessDotnetReloader(string pathToDotnetProject, string pathToDotnetUtil = "dotnet")
        {
            AppProcess.StartInfo.FileName = pathToDotnetUtil;
            AppProcess.StartInfo.Arguments = $"build {pathToDotnetProject}";
            InitProcess();
        }

        public override async Task<bool> Execute(int timeMilliseconds)
        {
            var result = false;
            await Task.Run(() =>
            {
                using (AppProcess)
                {
                    InitExecute();
                    result = AppProcess.WaitForExit(timeMilliseconds);
                    if (!String.IsNullOrEmpty(Error.ReadToEnd()))
                    {
                        result = false;
                    }
                }
            });
            return result;
        }
    }
}
