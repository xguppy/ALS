using System;
using System.Collections.Generic;

namespace ALS.CheckModule.Compare.DataStructures
{
    /// <summary>
    /// Результаты тестового прогона
    /// </summary>
    [Serializable]
    public struct ResultRun
    {
        /// <summary>
        /// Затраченная память
        /// </summary>
        public long Memory { get; set; }
        /// <summary>
        /// Затраченное время
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// Корректность
        /// </summary>
        public bool IsCorrect { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Входные данные
        /// </summary>
        public List<string> Input { get; set; }
        /// <summary>
        /// Выходные данные
        /// </summary>
        public List<string> Output { get; set; }
    }
}