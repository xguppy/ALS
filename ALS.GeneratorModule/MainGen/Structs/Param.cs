using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generator.MainGen.Structs
{
    public class Param
    {
        public string Value { get; set; } // стандартное значение объекта-параметра
        public string Name { get; set; } // имя объекта-параметра

        public Dictionary<string, string> Data; // словарь со всеми значениями

        public string Key = default; // ключ - выбранное значение для объекта-параметра

        public Param(Dictionary<string, string> data, string name, string key)
        {
            Value = default;
            Data = data;
            Name = name;
            Key = key != default ? key : "0";/*key != default ? (Data.ContainsKey(key) ? key : "0") : "0"*/;
        }

        // получение наиболее подходящего значения
        public string GetBestData()
        {
            if (Data.Count < 1) return "";
            string result = Data.ElementAt(0).Value;
            if (Data.ContainsKey(Key))
                result = Data[Key];
            return result;
        }

        // установка готовых значений
        public void SetValue(List<(string,string)> values)
        {
            if (values.Count > 0)
            {
                Value = values[0].Item2;
                if (values.Count > 1)
                {
                    Data = new Dictionary<string, string>();
                    foreach (var item in values)
                    {
                        Data.Add(item.Item1, item.Item2);
                    }
                }                
            }
        }
        // получение наиболее подходящего значения
        public string GetField(string fieldName)
        {
            var k = fieldName.ToLower();
            var res = Value;

            if (k != default)
            {
                if (k == "все") res = GetAll();
                else
                {
                    try { res = Data[k]; }
                    catch (Exception) { }
                }
            }
            return res;
        }

        // получение всех доступных значений
        public string GetAll()
        {
            StringBuilder s = new StringBuilder("");
            for (int i = 0; i < 100 && i < Data.Count; i++)
            {
                var item = Data.ElementAt(i);
                s.Append($"{item.Value}, ");
            }
            if (Data.Count > 100)
                s.Append($"[...] , {Data.ElementAt(Data.Count-1).Value}");
            else
                s.Remove(s.Length - 2, 2);
            return s.ToString();
        }

        // перевод в строку
        public override string ToString()
        {
            StringBuilder s = new StringBuilder($"{{\n\tname = {Name}\n\tvalue = {Value}\n\tData = ");
            int counter = 0;
            foreach (var item in Data)
            {
                s.Append($"[{item.Key} - {item.Value}] ");
                if (++counter > 100)
                {
                    s.Append(" {...} ");
                    break;
                }
            }
            s.Append($"\n\tkey = {Key}\n}}\n");
            return s.ToString();
        }
    }
}
