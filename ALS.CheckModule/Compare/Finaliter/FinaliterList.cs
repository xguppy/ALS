using System.IO;

namespace ALS.CheckModule.Compare.Finaliter
{
    public class FinaliterList: ComponentList<IFinaliter>
    {
        protected override string GetPathToSource()
            => Path.Combine(ModuleGovernor.GetPathToModule(GetComponentName()));

        public override IFinaliter Get(string name)
        {
            return name == null ? null : Actions[name];
        }

        
    }
}