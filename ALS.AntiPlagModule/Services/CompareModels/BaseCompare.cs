using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public abstract class BaseCompare
    {
        /// <summary>
        /// Lexems to compare with second
        /// </summary>
        protected readonly ILexer FirstParam;
        /// <summary>
        /// Lexems for compare
        /// </summary>
        protected readonly ILexer SecondParam;

        public BaseCompare(ILexer firstParam, ILexer secondParam) => (FirstParam, SecondParam) = (firstParam, secondParam);

        /// <summary>
        /// Returns result of algorithm work
        /// </summary>
        /// <returns></returns>
        public abstract int Execute();
    }
}
