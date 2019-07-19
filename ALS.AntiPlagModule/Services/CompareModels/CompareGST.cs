using System.Collections.Generic;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public class CompareGST : BaseCompare
    {
        public const int DefaultMinLengthTokens = 10;

        public CompareGST(ICollection<int> firstParam, ICollection<int> secondParam) : base(firstParam, secondParam) { }

        /// <summary>
        /// marking data for GST algorithm
        /// </summary>
        /// <param name="baseTokens"></param>
        /// <returns>marked data for algorithm</returns>
        private List<(int Key, bool Value)> unmarkData(ICollection<int> baseTokens)
        {
            var res = new List<(int Key, bool Value)>();
            foreach (var token in baseTokens)
            {
                res.Add((Key: token, Value: false));
            }
            return res;
        }
        /// <summary>
        /// GST algorithm for list of tokens
        /// </summary>
        /// <param name="a">Tokens for compare</param>
        /// <param name="b">Tokens for compare</param>
        /// <returns></returns>
        private HashSet<int[]> GST (ICollection<int> a, ICollection<int> b, int MinimumMatchLength = DefaultMinLengthTokens)
        {
            List<(int Key, bool Value)> P = unmarkData(a); // Первый контейнер -- получаем пары (токен, маркировка)
            List<(int Key, bool Value)> T = unmarkData(b); // Вторйо контейнер -- по тому же принципу
            var tiles = new HashSet<int[]>();           // Тайлы -- наибольшие непересекающиеся подстроки
            var matches = new HashSet<int[]>();         // Совпадения в одной итерации

            int maxmatch;

            do
            {
                maxmatch = MinimumMatchLength; // Максимальная длина совпадения, которую отрабатываем
                                               // При больших MinimumMatchLength избавляемся от шума и случайных совпадений
                for (int p = 0; p < P.Count; ++p) // Для каждого токена из P
                {
                    for (int t = 0; t < T.Count; ++t) // И каждого токена из T
                    {
                        int j = 0; // Длина общей подстроки
                        while ((p + j < P.Count) && (t + j < T.Count) && P[p + j].Key == T[t + j].Key && !P[p + j].Value && !T[t + j].Value)
                        {
                            j++; // Увеличиваем счетчик пока подстроки равны
                        }
                        if (j == maxmatch) // Если равно maxmatch, то вставляем во множество
                        {
                            matches.Add(new[] {p, t, j});
                        }
                        else if (j > maxmatch) // если больше -- затираем множество, инифиализируем текущим значением, изменяем макс.длину
                        {
                            matches.Clear();
                            matches.Add(new[] {p, t, j});
                            maxmatch = j; // Затираем максимальную длину найденной строки
                        }
                    }
                }
                foreach (var match in matches) // Для каждого совпадения
                {
                    // Проверка на маркировку диапазона -- хватает и граничных значений, что устанавливает сложность O(1)
                    if (!P[match[0]].Value && !P[match[0] + match[2] - 1].Value) // то же можно и для T, но зачем?
                    {
                        for (int j = 0; j<match[2]; ++j) // для j от 0 до maxmatch
                        {
                            // Маркируем токены как использованные, добавляем в результирующее множество
                            var tmp = P[match[0] + j];
                            tmp.Value = true;
                            P[match[0] + j] = tmp;

                            tmp = T[match[1] + j];
                            tmp.Value = true;
                            T[match[1] + j] = tmp;

                            tiles.Add(match);
                        }
                    }
                }
             } while (maxmatch > MinimumMatchLength);

            return tiles;
        }

        /// <summary>
        /// Calculating of Levenshtein algorithm!
        /// </summary>
        /// <returns>Levenshtein alg value</returns>
        public override int Execute()
        {
            var tiles = GST(FirstParam, SecondParam);
            int res = 0;
            foreach (var elem in tiles)
                res += elem[2];
            return res;
        }
    }
}
