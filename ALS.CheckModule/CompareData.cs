using System;

namespace ALS.CheckModule
{
    [Serializable]
    public struct CompareData
    {
        public long Memory { get; set; }
        public long Time { get; set; }
        public bool IsCorrect { get; set; }
    }
}