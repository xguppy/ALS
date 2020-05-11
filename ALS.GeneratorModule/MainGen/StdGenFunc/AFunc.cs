using Generator.MainGen.Parametr;
using Generator.Parsing;
using System.Linq;
using System.Collections.Generic;

namespace Generator.MainGen.StdGenFunc
{
    // Асбтрактный класс - функция генератора
    public abstract class AFunc
    {
        private Parser _pr = new Parser();

        // использование анализатора для получение
        // списка аргументов функции 
        protected string[] GetArgs(string str)
        {
            var args = _pr.GetSeparatedArgs(str);
            return args.Select(arg => arg.Trim(' ', '\n', '\r')).ToArray();
        }
        // хеширование имени функции для более удобного вызова нужной функции
        public static int GetHashOfFunc(string name)
        {
            return name.Sum(ch => ch * 1337);
        }
        // абстрактный меод запуска функции
        public abstract string Run(Param param);
    }
}
