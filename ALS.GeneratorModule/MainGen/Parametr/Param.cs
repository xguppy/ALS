using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.MainGen.Parametr
{
    public class Param
    {
        //public Param Parent { get; set; } = null;
        //public bool AreParentsFound { get; set; } = false;
        public string Value { get; set; }
        public string Name { get; set; }
        public string RawData { get; set; }
        public int Position { get; set; }

        public Param(string raw, int pos, string name, List<Param> parametrs = null)
        {
            raw = SubstituteValues(raw, parametrs);
            RawData = raw;
            Value = raw;
            Position = pos;
            Name = name;
        }

        public string GetFuncName()
        {
            int s = RawData.IndexOf('#');
            if (s == -1) return "just string";
            int f = RawData.IndexOf('(');
            if (f == -1) return "just string";
            return RawData.Substring(s+1, f-s-1);
        }

        private string SubstituteValues(string raw, List<Param> parametrs)
        {
            if (parametrs == null) return raw;

            StringBuilder s = new StringBuilder(raw);
            foreach (var elem in parametrs)
            {
                s = s.Replace($"@{elem.Name}@", elem.Value);
            }

            return s.ToString();
        }

        public override string ToString()
        {
            return $"{Name} : {RawData}";
        }
    }
}
