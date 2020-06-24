using Generator.MainGen.Structs;
using System.Collections.Generic;
using NLua;

namespace Generator.MainGen.StdGenFunc
{
    // Асбтрактный класс - функция генератора
    public abstract class AFunc
    {
        // св-во, указывет на количество возвращаемых параметров
        protected bool MultypleReturnDatas = false;

        // шаблонный метод
        public List<(string, string)> Call(FunctionStruct fs)
        {
            // вовзвращаемые данные
            List<(string, string)> returningData = new List<(string, string)>();
            // вызов конкретного метода
            var result = Run(fs);
            // создание списка готовых значений
            if (MultypleReturnDatas) {
                if (result.ToString() == "table")
                {
                    LuaTable lt = (LuaTable)result;
                    foreach (var k in lt.Keys)
                        returningData.Add((k.ToString().ToLower(), lt[k].ToString()));
                }
                else
                {
                    return result;
                }
            }
            else{
                returningData.Add(("0", result));
            }

            return returningData;
        }
        public abstract dynamic Run(FunctionStruct fs);
    }
}
