using System;
using System.IO;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule.Compare
{
    public static class ModuleGovernor
    {
        private static bool _readyToBuild = true;
        
        public static string GetPathToModule(string nameComponent) => Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, $"ALS.CheckModule.Component.{nameComponent}");
        public static void AllowBuild() => _readyToBuild = true;
        public static async Task<bool> BuildModule(string nameComponent)
        {
            if (!_readyToBuild)
            {
                throw new Exception("В данный момент система производит сборку");
            }
            _readyToBuild = false;
            var modulePath = GetPathToModule(nameComponent);
            var dotnetAssembly = new ProcessDotnetReloader(modulePath);
            var successfulBuild = await dotnetAssembly.Execute(10000);
            return successfulBuild;
        }
    }
}