using Generator.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generator.MainGen.Parametr
{
    public class ParamsContainer : IParamsContainer
    {
        public List<Param> Parametrs { get; set; } = new List<Param>();
        public GenFunctions Gf = new GenFunctions();

        public List<Param> GenNewParametrs(List<DataContainer> d)
        {
            Random r = new Random();
            Parametrs.Clear();
            for (int counter = 0; counter < d.Count; counter++)
            {
                DataContainer sd = d[counter];
                bool flag = false;
                Dictionary<string, int> map = new Dictionary<string, int>();
                for (int i = 0; i < sd.Data.Count; i++)
                {
                    map.Add(sd.Data[i], i+1);
                }
                Param param = new Param("INIT", 0, sd.Name);
                while (!flag && sd.Data.Count > 0)
                {
                    int pos = r.Next(0, sd.Data.Count);
                    string rawData = sd.Data[pos];
                    sd.Data.RemoveAt(pos);
                    param = new Param(rawData, map[rawData], sd.Name, Parametrs);
                    if ( param.WhatIsIt() == FuncsEnum.parent)
                    {
                        if (!bool.Parse(Gf.WhatToDoWithParam(FuncsEnum.parent, param, Parametrs))) continue;
                    }
                    param.Value = Gf.WhatToDoWithParam(param.WhatIsIt(), param, Parametrs);
                    flag = true;
                }

                if (!flag && sd.Data.Count == 0)
                {
                    //param = new Param($"--< ERROR var({sd.Name}) - cannot gen a value for that variable >--", 0, sd.Name);
                    param = new Param("", 1, sd.Name);
                }
                // здесь добавить чтоб изменял параметры с одинаковыми именами
                Parametrs.Add(param);
            }

            return Parametrs;
        }
    }
}
