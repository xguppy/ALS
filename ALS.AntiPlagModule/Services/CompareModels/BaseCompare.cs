using System.Collections.Generic;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public abstract class BaseCompare
    {
        /// <summary>
        /// Lexems to compare with second
        /// </summary>
        protected readonly ICollection<int> FirstParam;
        /// <summary>
        /// Lexems for compare
        /// </summary>
        protected readonly ICollection<int> SecondParam;

        public BaseCompare(ICollection<int> firstParam, ICollection<int> secondParam) => (FirstParam, SecondParam) = (firstParam, secondParam);

        /// <summary>
        /// Returns result of algorithm work
        /// </summary>
        /// <returns></returns>
        public abstract int Execute();
    }
}
