using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ALS.CheckModule.Compare
{
    [Serializable]
    public struct CompareData
    {
        [DefaultValue(512000)]
        public long Memory { get; set; }
        [DefaultValue(10000)]
        public int Time { get; set; }
        public bool IsCorrect { get; set; }
    }
}