using System.IO;

namespace ALS.CheckModule.Compare.Finaliter
{
    public class FinaliterList: ActionList<IFinaliter>
    {
        protected override string GetPathToSource()
            => Path.Combine(GetPathToModule(), "Compare", "Finaliter");

        public override IFinaliter Get(string name)
        {
            return name == null ? null : Actions[name];
        }
    }
}