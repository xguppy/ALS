using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generator.MainGen.Structs
{
    public class Services
    {
        private const string _arithmSigns = "знаки_арифм";
        private const string _arithmFuncs = "функции_арифм";
        public Elems Elems { get; set; }

        public void InitDefault()
        {
            Elems = new Elems();
            Elems.Default();
        }

        public void Init(List<Param> Structss)
        {
            Elems = new Elems();
            foreach (Param p in Structss)
            {
                switch (p.Name.ToLower())
                {
                    case _arithmFuncs:
                        Elems.SetFuncs(p.Data.Select(x => x.Value).ToList());
                        break;
                    case _arithmSigns:
                        Elems.SetSigns(p.Data.Select(x => x.Value).ToList());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
