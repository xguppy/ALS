using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public interface IModelComparable
    {
        float Execute(ICollection<int> firstTokens, ICollection<int> secondTokens);
    }
}
