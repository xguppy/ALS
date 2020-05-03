using System;
using System.IO;

namespace ALS.CheckModule.Compare.Checker
{
    public class CheckerList: ComponentList<IChecker>
    {
        protected override string TemplateComponent { get; set; } = "AbsoluteChecker.cs";

        protected override string GetPathToSource()
            => Path.Combine(ModuleGovernor.GetPathToModule(GetComponentName()));
        
        
    }
}
