using Generator.MainGen;
using Generator.MainGen.Structs;
using Generator.MainGen.StdGenFunc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Generator.Parsing
{
    public class Parser
    {
        // список секций
        private const string _storage = "ХРАНИЛИЩЕ_ОБЪЕКТОВ";
        private const string _template = "ШАБЛОННЫЙ_ВИД";
        private const string _solution = "РЕШЕНИЕ";
        private const string _service = "СЛУЖЕБНОЕ";
        private const string _tests = "ТЕСТОВЫЕ_ДАННЫЕ";
        // словарь с доступными функциями
        private Dictionary<string, FuncsEnum> _funcs = new Dictionary<string, FuncsEnum>();
        
        private const char _or = '|';
        private const char _super = (char)254;

        public Parser()
        {
            // инициализация словаря с функциями
            foreach (int i in Enum.GetValues(typeof(FuncsEnum)))
            {
                FuncsEnum f = (FuncsEnum)i;
                _funcs.Add(f.ToString(), f);
            }            
        }
        // разделение секций
        private string[] SplitText(string text)
        {
            string lines = "----";
            Regex r = new Regex("\\/\\/.*");
            text = r.Replace(text, "");
            r = new Regex($"{lines}(\\W*)\n");
            text = r.Replace(text, lines);
            var p = text.Split(lines).Select(str => str.Trim(' ', '\n', '\r')).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            return p;
        }
        // Получение списка (название секции - содержимое)
        public List<(BlockEnum, StringBuilder)> GetBlocks(string fileName)
        {
            List<(BlockEnum, StringBuilder)> result = new List<(BlockEnum, StringBuilder)>();
            try
            {
                string text;
                // чтение файла
                using (StreamReader s = new StreamReader(fileName)) text = s.ReadToEnd();
                // разделение файла наблоки
                var splitedText = SplitText(text);
                // обработка блоков
                foreach (var str in splitedText)
                {
                    var b = Block(str);
                    result.Add((b.Item1, new StringBuilder(b.Item2)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Возникла ошибка во время парсинга файла! Сообщение об ошибке = [{ex.Message}] ");
            }

            return result;
        }

        // определение типа секции
        private (BlockEnum, string) Block(string text)
        {
            var pos = text.IndexOf('\n');
            var head = text.Substring(0, pos);
            var body = text.Remove(0, pos);
            switch (head.Trim(' ', '\r').ToUpper())
            {
                case _storage:
                    return (BlockEnum.Storage, body);
                case _template:
                    return (BlockEnum.Template, body);
                case _service:
                    return (BlockEnum.Service, body);
                case _solution:
                    return (BlockEnum.Solution, body);
                case _tests:
                    return (BlockEnum.Tests, body);
                default:
                    return (BlockEnum.Text, text);
            }
        }

        private bool CheckShielding(StringBuilder s, int i)
        {
            return (i > 0 && s[i - 1] == '\\');
        }
        // индекс символа в StringBuilder
        private int IndexOf(StringBuilder s, char ch)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ch && !CheckShielding(s, i))
                {
                    return i;
                }
                else if (s[i] == '\"')
                {
                    i = FindStringEnd(s, i, '\"');
                    if (i < 0) throw new Exception($"Не получилось найти окончание строки: [{s.ToString()}]");
                }
            }
            return -1;
        }
        // получение очередного объекта-параметра в секции
        public bool GetParamString(StringBuilder text, out string outputParam)
        {
            outputParam = default;
            var pos = IndexOf(text, ';');
            if (pos == -1) return false;
            outputParam = text.ToString(0, pos).Trim();
            text.Remove(0, pos+1);
            return outputParam != default;
        }
        // создание словаря (ключ-значение) всех значений присвоенных объекту-параметру
        private Dictionary<string, string> CreateDictionaryFromRaw(string[] raw)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            int iter = 0;
            // для каждого значения
            foreach (var str in raw)
            {
                // выбирается наиболее подходящий ключ
                var res = GetAssociativeValues(str);
                var key = res.Item1 != default ? res.Item1 : iter.ToString();
                // если ключ не был установлен явно, то используется просто номер
                if (res.Item1 == default) iter++;
                d.Add(key, res.Item2);
            }

            return d;
        }
        // создание сырого параметра, все значения находятся в первоначальном виде
        public Param CreateRawParam(string paramStr)
        {
            Param p = null;
            var parts = paramStr.Split(':', 2);
            if (parts.Length > 1)
            {
                var rawName = GetAssociativeValues(parts[0]);
                var raw  = GetSeparatedValues(parts[1], _or);
                //            список всех значений          имя параметра  выбранное значение
                p = new Param(CreateDictionaryFromRaw(raw), rawName.Item2, rawName.Item1);
            }

            return p;
        }
        // получение имени функции
        private string GetFuncName(string value)
        {
            int s = value.IndexOf('#');
            if (s == -1 || (s > 0 && value[s-1] == '\\')) return FuncsEnum.justString.ToString();
            int f = value.IndexOf('(');
            if (f == -1 || (f > 0 && value[f - 1] == '\\')) return FuncsEnum.justString.ToString();
            return value.Substring(s + 1, f - s - 1);
        }
        // определяем тип функции
        private FuncsEnum WhatFunctionType(string name)
        {
            if (name == FuncsEnum.justString.ToString()) return FuncsEnum.justString;

            try
            {
                return _funcs[name];
            }
            catch (Exception)
            {
                return FuncsEnum.расширениеЛуа;
            }
            /*int h = AFunc.GetHashOfFunc(name);
            if (_funcs.ContainsKey(h))
            {
                if (name == $"{_funcs[h]}")
                    return _funcs[h];
            }*/
        }
        // восстановление параметров из списка
        public string RestoreArgs(List<string> args)
        {
            StringBuilder funcArgs = new StringBuilder("");
            if (args.Count > 0)
            {
                for (int i = 0; i < args.Count; i++)
                    funcArgs.Append($",{args[i]}");
                funcArgs[0] = ' ';
            }
            return funcArgs.ToString();
        }        
        public FunctionStruct CreateFunctionStruct(string value)
        {
            string fun = GetFuncName(value);
            var funType = WhatFunctionType(fun);
            var listArgs = funType!= FuncsEnum.justString ? GetSeparatedArgs(value) : null;
            var args = funType != FuncsEnum.justString ? RestoreArgs(listArgs) : value;
            return new FunctionStruct(fun, value, args, listArgs, funType);
            //FunctionStruct fs = new FunctionStruct(fun, value, args, listArgs, funType);
            //return fs;
        }

        // получение двух ключ значение из записи типа "[ключ] значение"
        public (string, string) GetAssociativeValues(string str)
        {
            return GetQuotedValues(str, '[', ']');
        }

        public (string, string) GetQuotedValues(string s, char bracketS, char bracketE)
        {
            string key = default, value = s;
            var start = s.IndexOf(bracketS);
            if (start >= 0)
            {
                var end = FindStringEnd(new StringBuilder(s), start, bracketE);
                if (end < 0) throw new Exception($"{s} - отсутствует символ окончания {bracketE}");
                key = s.Substring(start + 1, end - start - 1).Trim();
                value = s.Remove(start, end - start + 1);
            }

            return (key, value.Trim('\n', ' '));
        }

        // поиск строк в формате "..."
        public int FindStringEnd(StringBuilder s, int pos, char separator)
        {
            pos++;
            for (; pos < s.Length; pos++)
            {
                if (s[pos] == separator)
                {
                    if (pos > 0 && s[pos - 1] == '\\')
                    {
                        s[pos - 1] = ' ';
                    }
                    else
                    {
                        return pos;
                    }
                }
            }
            return -1;
        }
        // проверка на экранирование
        private void HandleOfSymbol(StringBuilder s, int i)
        {
            if (i > 0 && s[i - 1] == '\\')
                s.Remove(i - 1, 1);
            else
                s[i] = _super;
        }
        // получения списка значений разделенных символом separator
        public string[] GetSeparatedValues(string str, char separator)
        {
            StringBuilder s = new StringBuilder(str);

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == separator)
                {
                    HandleOfSymbol(s, i);
                }
                else if (s[i] == '\"')
                {
                    i = FindStringEnd(s, i, '\"');
                    if (i < 0)
                        throw new Exception($"Не получилось найти окончание строки: [{s}]");
                }
            }

            return s.ToString().Split(_super, StringSplitOptions.RemoveEmptyEntries);
        }

        // получение списка аргументов передаваемых в функции
        public List<string> GetSeparatedArgs(string str, bool split = true)
        {
            const char separator = ',';
            int i_start = str.IndexOf('(') + 1;
            if (i_start < 0) throw new Exception($"Не получилось найти окончание функции: [{str}]");
            StringBuilder s = new StringBuilder(str.Substring(i_start));
            int counter = 1;

            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '(':
                        counter++;
                        break;
                    case ')':
                        counter--;
                        if (counter < 1)
                        {
                            s = s.Remove(i, s.Length - i);
                            i = s.Length;
                        }
                        break;
                    case '\"':
                        i = FindStringEnd(s, i, '\"');
                        if (i < 0) throw new Exception($"Не получилось найти окончание строки: [{str}]");
                        break;
                    case separator:
                        if (counter == 1)
                        {
                            if (i > 0 && s[i - 1] == '\\')
                                s = s.Remove(i - 1, 1);
                            else if (split)
                                s[i] = _super;
                        }
                        break;
                }
            }

            if (counter > 0) throw new Exception($"Не получилось найти окончание функции: [{str}]");

            return s.ToString().Split(_super, StringSplitOptions.RemoveEmptyEntries).Select(arg => arg.Trim(' ', '\n', '\r')).ToList();
        }
    }
}
