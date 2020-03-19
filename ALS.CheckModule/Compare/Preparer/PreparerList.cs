using System.IO;

namespace ALS.CheckModule.Compare.Preparer
{
    public class PreparerList: ActionList<IPreparer>
    {
        protected override string GetPathToSource()
            => Path.Combine(GetPathToModule(), "Compare", "Preparer");

        public override IPreparer Get(string name)
        {
            return name == null ? null : Actions[name];
        }
    }
}