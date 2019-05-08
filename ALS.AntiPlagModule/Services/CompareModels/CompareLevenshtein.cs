using System;
using System.Collections.Generic;
using System.Linq;

namespace ALS.AntiPlagModule.Services.CompareModels
{
    public class CompareLevenshtein : BaseCompare
    {
        public CompareLevenshtein(ICollection<int> firstParam, ICollection<int> secondParam) : base(firstParam, secondParam) { }

        /// <summary>
        /// Calculating of Levenshtein algorithm!
        /// </summary>
        /// <returns>Levenshtein alg value</returns>
        public override int Execute()
        {
            // Алгоритм расстояния Левенштейна
            int m = FirstParam.Count;  // Размер исходного контейнера
            int n = SecondParam.Count; // Размер контейнера, с которым сравниваем
            int value_algorithm = 0;     // Число, полученное из алгоритма Левенштейна

            if (m != 0 && n != 0)
            {
                // Результирующая матрица
                var matrix = new int[m + 1, n + 1];

                // По левому и верхнему краю значения от 0 до (m и n соотв.)
                for (int i = 0; i <= m; ++i)
                {
                    matrix[i, 0] = i;
                }
                for (int i = 0; i <= n; ++i)
                {
                    matrix[0, i] = i;
                }
                
                int cost;
                // Проход по матрице
                for (int i = 1; i <= m; ++i)
                {
                    for (int j = 1; j <= n; ++j)
                    {
                        cost = FirstParam.ToArray()[i - 1] == SecondParam.ToArray()[j - 1] ? 0 : 1; // Если равны, ничего не присваиваем, иначе стоимость равна 1
                        matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), matrix[i - 1, j - 1] + cost);
                    }
                }
                value_algorithm = matrix[m, n]; // Результат -- последний элемент
                                                //result = 1.0 - (1.0 * value_algorithm / std::max(m, n)); // Максимум -- длина наибольшей строки. Вычитаем из 1, так как при 1 у нас совпадений нет)
            }

            return value_algorithm;
        }
    }
}
