using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using ALS.CheckModule.Processes;

namespace ALS.CheckModule.Compare.Checker
{
    public class CheckerList: ActionList<IChecker>
    {
        protected override string GetPathToSource()
            => Path.Combine(GetPathToModule(), "Compare", "Checker");

        public override IChecker Get(string name)
        {
            //Если чекер не задан применим дефолтный
            if (name == null)
            {
                name = "AbsoluteChecker";
            }
            //Если чекера нет в словаре бросим исключение
            if (!Actions.ContainsKey(name))
            {
                throw new Exception("Выбранного чекера не существует");
            }
            return Actions[name];
        }
    }
}
