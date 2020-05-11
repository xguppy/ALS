using Generator.MainGen.StdGenFunc;
using Generator.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.MainGen.Parametr
{
    public class ParamsContainer
    {
        public List<Param> Parametrs { get; set; } = new List<Param>();
        public List<Param> ParametrsForTest { get; set; } = new List<Param>();
        public GenFunctions Gf = new GenFunctions();
        private Dictionary<int, FuncsEnum> _funcs = new Dictionary<int, FuncsEnum>();
        Parser _pr = new Parser();

        public ParamsContainer()
        {
            foreach (int i in Enum.GetValues(typeof(FuncsEnum)))
            {
                FuncsEnum f = (FuncsEnum)i;
                _funcs.Add(AFunc.GetHashOfFunc(f.ToString()), f);
            }
        }

        private FuncsEnum WhatParamIsIt(string name)
        {
            if (name == FuncsEnum.justString.ToString()) return FuncsEnum.justString;

            int h = AFunc.GetHashOfFunc(name);
            if (_funcs.ContainsKey(h))
            {
                if (name == $"{_funcs[h]}")
                    return _funcs[h];
            }
            return FuncsEnum.luaExtension;
        }

        private string InitBestValue(DataContainer d, string key)
        {
            string value = default;
            try
            {
                var i = int.Parse(key);
                value = d.Data[i];
            }
            catch (Exception)
            {
                value = d.Data[0];
            }
            return value;
        }

        private (Param, bool) FirstInit(DataContainer d, List<Param> parametrs)
        {
            Param param = new Param(default, default, d.Name, parametrs);
            var keyAndValue = _pr.GetAssociativeValues(param.Name);
            bool flag = keyAndValue.Item1 != default;
            if (flag)
            {
                string raw = default;
                bool isValueGot = false;
                for (int i = 0; i < d.Data.Count&& !isValueGot; i++)
                {
                    var item = d.Data[i];
                    var p = _pr.GetAssociativeValues(item);
                    d.Data[i] = p.Item2;
                    if (p.Item1 == keyAndValue.Item1)
                    {
                        raw = p.Item2;
                        isValueGot = true;
                    }
                }
                if (!isValueGot) raw = InitBestValue(d, keyAndValue.Item1);
                param.Init(raw, default, keyAndValue.Item2, parametrs);
            }

            return (param, !flag);
        }

        public List<Param> GenNewParametrs(List<DataContainer> dataContainer)
        {
            Random r = new Random();
            Parametrs.Clear();
            foreach (DataContainer d in dataContainer)
            {
                var inited = FirstInit(d, Parametrs);
                var param = inited.Item1;
                /*  Dictionary<string, int> map = new Dictionary<string, int>();
                    for (int i = 0; i < d.Data.Count; i++)
                        map.Add(d.Data[i], i + 1);
                    while (d.Data.Count > 0)
                    {
                        int pos = r.Next(0, d.Data.Count);
                        string rawData = d.Data[pos];
                        d.Data.RemoveAt(pos);
                        param = new Param(rawData, map[rawData], d.Name, Parametrs);
                        var ftype = WhatParamIsIt(param.GetFuncName());
                        param.Value = Gf.WhatToDoWithParam(ftype, param);
                    }*/
                if (inited.Item2)
                {
                    int pos = r.Next(0, d.Data.Count);
                    param.Init(d.Data[pos], pos + 1, d.Name, Parametrs);
                }
                var ftype = WhatParamIsIt(param.GetFuncName());
                param.Value = Gf.WhatToDoWithParam(ftype, param);

                Parametrs.Add(param);
            }

            return Parametrs;
        }

        public List<Param> Tests(List<DataContainer> dataContainer)
        {
            Random r = new Random();
            foreach (DataContainer d in dataContainer)
            {
                var inited = FirstInit(d, ParametrsForTest);
                var param = inited.Item1;
                StringBuilder s = new StringBuilder();
                foreach (var item in d.Data)
                {
                    if (inited.Item2)
                    {
                        param.Init(item, default, d.Name, ParametrsForTest);
                    }
                    var ftype = WhatParamIsIt(param.GetFuncName());
                    s.Append(", ");
                    s.Append(Gf.WhatToDoWithParam(ftype, param));
                }
                s.Remove(0, 2);
                param.Value = s.ToString();
                ParametrsForTest.Add(param);
            }

            return ParametrsForTest;
        }

    }
}
