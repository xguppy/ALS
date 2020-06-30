using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.MainGen.Structs
{
    public class Test
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public List<string> Data { get; set; } = new List<string>();
    }
}
