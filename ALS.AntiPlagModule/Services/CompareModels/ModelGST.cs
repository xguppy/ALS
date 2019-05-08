using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public class ModelGST: IModelComparable
    {
        public float Execute(ICollection<int> firstTokens, ICollection<int> secondTokens)
        {
            var f = new CompareGST(firstTokens, secondTokens);
            return 1.0f * f.Execute() / Math.Min(firstTokens.Count, secondTokens.Count);
        }
    }
}
