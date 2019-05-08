using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public class ModelLevenshtein: IModelComparable
    {
        public float Execute(ICollection<int> firstTokens, ICollection<int> secondTokens)
        {
            var f = new CompareLevenshtein(firstTokens, secondTokens);
            return 1.0f - (1.0f * f.Execute() / Math.Max(firstTokens.Count, secondTokens.Count));
        }
    }
}
