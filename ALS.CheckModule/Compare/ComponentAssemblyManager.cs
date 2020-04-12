using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ALS.CheckModule.Compare
{
    public class ComponentAssemblyManager
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Execute(out WeakReference testAlcWeakRef, out CheckModuleLoadContext alc, string pathToAssembly)
        {
            alc = new CheckModuleLoadContext();
            testAlcWeakRef = new WeakReference(alc);
            return alc.LoadFromAssemblyPath(pathToAssembly); ;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Unload(WeakReference testAlcWeakRef, CheckModuleLoadContext alc)
        {
            alc.Unload();
            alc = null;
            await Task.Run(() =>
            {
                for (int i = 0; testAlcWeakRef.IsAlive && (i < 15); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            });
        }
    }
}