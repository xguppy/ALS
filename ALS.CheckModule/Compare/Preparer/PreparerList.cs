using System.IO;

namespace ALS.CheckModule.Compare.Preparer
{
    public class PreparerList: ComponentList<IPreparer>
    {
        protected override string GetPathToSource()
            => Path.Combine(ModuleGovernor.GetPathToModule(), "Compare", "Preparer");

        public override IPreparer Get(string name)
        {
            return name == null ? null : Actions[name];
        }
    }
}