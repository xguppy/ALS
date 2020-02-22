using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Generator.MainGen.Parametr;
using Generator.Parsing;
using Newtonsoft.Json;
using System.Text;
using NLua;
using Generator.MainGen.ForGenFunc;

namespace Generator.MainGen
{
    public class GenFunctions
    {
        private Dictionary<FuncsEnum, AFunc> _f = new Dictionary<FuncsEnum, AFunc>();

        public GenFunctions()
        {
            _f.Add(FuncsEnum.rnd, new Rnd());
            _f.Add(FuncsEnum.genAE, new GenExpr());
            _f.Add(FuncsEnum.lua, new LuaFunc());
            _f.Add(FuncsEnum.parent, new ParentChecker());
        }

        public string WhatToDoWithParam(FuncsEnum funcs, Param param, List<Param> parametrs)
        {
            switch (funcs)
            {
                case FuncsEnum.justString:
                    return param.RawData;

                case FuncsEnum.getAEcode:
                    return ((GenExpr)_f[FuncsEnum.genAE]).ExpressionCodeOnC();

                default:
                    return _f[funcs].Run(param, parametrs);

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

        public List<List<string>> GetTestsFromJson(string json)
        {
            List<List<string>> result = new List<List<string>>();

            var tests = JsonConvert.DeserializeObject<List<DataContainer>>(json);

            foreach (var dc in tests)
            {
                if (dc.Data.Count > 1)
                {
                    result.Add(dc.Data);
                }
                else
                {
                    if (dc.Data[0].Contains($"#{FuncsEnum.rnd}"))
                    {
                        Param p = new Param(dc.Data[0], -1, "TEMP");
                        result.Add(new List<string>(_f[0].Run(p).Split(',')));
                    }
                    else
                    {
                        result.Add(dc.Data);
                    }
                }
            }

            return result;
        }
    }
}