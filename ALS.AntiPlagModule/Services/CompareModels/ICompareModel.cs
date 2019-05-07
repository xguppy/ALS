using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public interface ICompareModel
    {
        float Execute(BaseCompare compare);
    }
}
