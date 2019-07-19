using System.Collections.Generic;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public interface IModelComparable
    {
        float Execute(ICollection<int> firstTokens, ICollection<int> secondTokens);
    }
}
