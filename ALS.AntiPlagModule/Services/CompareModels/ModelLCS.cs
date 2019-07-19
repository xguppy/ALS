using System;
using System.Collections.Generic;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public class ModelLCS: IModelComparable
    {
        public float Execute(ICollection<int> firstTokens, ICollection<int> secondTokens)
        {
            var f = new CompareLCS(firstTokens, secondTokens);
            return 1.0f * f.Execute() / Math.Min(firstTokens.Count, secondTokens.Count);
        }
    }
}
