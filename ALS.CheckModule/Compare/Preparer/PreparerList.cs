using System.IO;

namespace ALS.CheckModule.Compare.Preparer
{
    public class PreparerList: ComponentList<IPreparer>
    {
        protected override string TemplateComponent { get; set; } = "AbsolutePreparer.cs";

        protected override string GetPathToSource()
            => Path.Combine(ModuleGovernor.GetPathToModule(GetComponentName()));

        public override IPreparer Get(string name)
        {
            return name == null ? null : Actions[name];
        }
    }
}