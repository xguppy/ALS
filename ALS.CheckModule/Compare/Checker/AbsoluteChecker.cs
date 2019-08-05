using System.Collections.Generic;
using ALS.CheckModule.Compare.DataStructures;

namespace ALS.CheckModule.Compare.Checker
{
    public class AbsoluteChecker: IChecker
    {
        public void Check(List<string> modeOutput, ref ResultRun result)
        {
            var counter = 0;
            
            foreach (var userElem in result.Output)
            {
                if (userElem != modeOutput[counter])
                {
                    result.Comment = $"Ошибка на позиции {counter}: ожидалось {modeOutput[counter]}, получено {userElem}";
                    return;
                }
                ++counter;
            }

            if (counter < modeOutput.Count)
            {
                result.Comment = $"Количество выводов в программе {counter} ожидалось {modeOutput.Count}";
                return;
            }

            result.Comment = "Всё хоккей";
            result.IsCorrect = true;
        }
    }
}