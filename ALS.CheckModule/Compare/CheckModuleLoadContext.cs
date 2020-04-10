using System.Reflection;
using System.Runtime.Loader;

namespace ALS.CheckModule.Compare
{
    public class CheckModuleLoadContext: AssemblyLoadContext
    {
        public CheckModuleLoadContext() : base(isCollectible: true)
        { }
 
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}