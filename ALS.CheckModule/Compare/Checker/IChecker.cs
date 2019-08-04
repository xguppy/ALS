using System.Collections.Generic;
using ALS.CheckModule.Compare.DataStructures;

namespace ALS.CheckModule.Compare.Checker
{
    public interface IChecker
    {
        /// <summary>
        /// Метод проверки одного тестового прохода
        /// </summary>
        /// <param name="userOutput">Выходной поток пользователя</param>
        /// <param name="modeOutput">Эталонный выходной поток</param>
        /// <param name="result">Результат тестового прохода(изменяются IsCorrect и Comment)</param>
        void Check(List<string> userOutput, List<string> modeOutput, ref ResultRun result);
    }
}