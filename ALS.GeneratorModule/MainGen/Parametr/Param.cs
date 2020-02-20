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

        public string GetFuncName(string str)
        {
            int p = str.IndexOf('(');
            if (p < 0) return "NULL";
            return str.Substring(0, p);
        }

        public FuncsEnum CheckParamType(string str)
        {
            string nameOfFunction = GetFuncName(str);
            FuncsEnum type = FuncsEnum.justString;

            if (nameOfFunction == $"#{FuncsEnum.rnd}")
            {
                type = FuncsEnum.rnd;
            }
            else if (nameOfFunction == $"#{FuncsEnum.genAE}")
            {
                type = FuncsEnum.genAE;
            }
            else if (nameOfFunction == $"#{FuncsEnum.getAEcode}")
            {
                type = FuncsEnum.getAEcode;
            }
            else if (nameOfFunction == $"#{FuncsEnum.lua}")
            {
                type = FuncsEnum.lua;
            }
            else if (nameOfFunction == $"#{FuncsEnum.parent}")
            {
                type = FuncsEnum.parent;
            }

            return type;
        }

        public FuncsEnum WhatIsIt()
        {
            return CheckParamType(RawData);
        }

        private string SubstituteValues(string raw, List<Param> parametrs)
        {
            if (parametrs == null) return raw;

            StringBuilder s = new StringBuilder(raw);
            foreach (var elem in parametrs)
            {
                s = s.Replace($"({elem.Name})", elem.Value);
                /*if (args[i].Contains($"({elem.Name})"))
                {
                    args[i] = args[i].Replace($"({elem.Name})", elem.Value);
                }*/
            }

            return s.ToString();
        }

        /*public FuncsEnum FindParent()
        {
            if (RawData.Contains($"#{FuncsEnum.parent}"))
            {
                return FuncsEnum.parent;
            }

            return FuncsEnum.justString;
        }*/
    }
}
