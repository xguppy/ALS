using System.Collections.Generic;
using System.Threading.Tasks;
using ALS.CheckModule.Compare.DataStructures;

namespace ALS.CheckModule.Compare
{
    public class Verification
    {
        private string _userProg;
        private string _modelProg;
        private Constrains _constrains;

        public Verification(string userProg, string modelProg, Constrains constrains)
        {
            _userProg = userProg;
            _modelProg = modelProg;
            _constrains = constrains;
        }

        public async Task<List<ResultRun>> RunTests(List<List<string>> inputTestSets)
        {
            var results = new List<ResultRun>();
            
            foreach (var elem in inputTestSets)
            {
                var cmp = new Comparer(_modelProg, _userProg, elem);
                var dataRun = await cmp.CompareAsync(_constrains);
                results.Add(dataRun);
            }

            return results;
        }
    }
}