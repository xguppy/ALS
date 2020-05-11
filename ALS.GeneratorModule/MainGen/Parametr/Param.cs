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
        public string FunctionName { get; set; }
        public int Position { get; set; } // DEPRECATED

        public Param(string raw, int pos, string name, List<Param> parametrs = null)
        {
            Init(raw, pos, name, parametrs);
            FunctionName = default;
        }

        public void Init(string raw, int pos, string name, List<Param> parametrs = null)
        {
            raw = SubstituteValues(raw, parametrs);
            name = SubstituteValues(name, parametrs);
            RawData = raw;
            Value = raw;
            Position = pos; // DEPRECATED
            Name = name;
        }

        public string GetFuncName()
        {
            int s = RawData.IndexOf('#');
            if (s == -1) return FuncsEnum.justString.ToString();
            int f = RawData.IndexOf('(');
            if (f == -1) return FuncsEnum.justString.ToString();
            FunctionName = RawData.Substring(s + 1, f - s - 1);
            return FunctionName;
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
            return $"{Name} : {Value}";
        }
    }
}
