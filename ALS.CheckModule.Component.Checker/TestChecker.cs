using System.Collections.Generic;
using ALS.CheckModule.Compare.DataStructures;

namespace ALS.CheckModule.Compare.Checker
{
    public class TestChecker: IChecker
    {
        public void Check(List<string> modeOutput, string pathToUserProgram, string pathToModelProgram, ref ResultRun result)
        {
            result.IsCorrect = true;
        }
    }
}