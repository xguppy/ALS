using System.Collections.Generic;
using Generator.MainGen.Parametr;
using Generator.Parsing;
using Newtonsoft.Json;
using Generator.MainGen.StdGenFunc;
using System.Linq;

namespace Generator.MainGen
{
    public class GenFunctions
    {
        private Dictionary<FuncsEnum, AFunc> _f = new Dictionary<FuncsEnum, AFunc>();
        // инициализация функций
        public GenFunctions()
        {
            _f.Add(FuncsEnum.rnd, new Rnd());
            _f.Add(FuncsEnum.genAE, new GenExpr());
            _f.Add(FuncsEnum.lua, new LuaFunc());
            _f.Add(FuncsEnum.parent, new ParentChecker());
            _f.Add(FuncsEnum.luaExtension, new LuaExtension());
        }
        // исполнение нужной функции генератора
        public string WhatToDoWithParam(FuncsEnum funcs, Param param)
        {
            switch (funcs)
            {
                case FuncsEnum.justString:
                    //return param.RawData;
                    return param.Value;

                case FuncsEnum.getAEcode:
                    return ((GenExpr)_f[FuncsEnum.genAE]).ExpressionCodeOnC();

                default:
                    return _f[funcs].Run(param);
            }
        }

        public bool CheckTests(List<DataContainer> lDc)
        {
            foreach (DataContainer dc in lDc)
            {
                if (dc.Data.Count == 1) continue;

                foreach (string str in dc.Data)
                {
                    if (str.Contains($"#{FuncsEnum.rnd}"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public List<Param> GetTestsFromJsonNewVersion(string json)
        {
            var tests = JsonConvert.DeserializeObject<List<DataContainer>>(json);
            ParamsContainer p = new ParamsContainer();
            return p.Tests(tests);
        }

        public List<(string, List<string>)> GetTestsFromJson(string json)
        {
            var tests = JsonConvert.DeserializeObject<List<DataContainer>>(json);
            ParamsContainer p = new ParamsContainer();
            var t = p.Tests(tests);
            return t.Select(p => (p.Name, p.Value.Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList())).ToList();
        }
    }
}