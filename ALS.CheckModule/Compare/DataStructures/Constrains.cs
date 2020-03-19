using System;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace ALS.CheckModule.Compare.DataStructures
{
    /// <summary>
    /// Ограничения
    /// </summary>
    [Serializable]
    public struct Constrains
    {
        /// <summary>
        /// Память в байтах
        /// </summary>
        [DefaultValue(512000)]
        public long Memory { get; set; }
        /// <summary>
        /// Время в мс
        /// </summary>
        [DefaultValue(10000)]
        public int Time { get; set; }
        /// <summary>
        /// Подготавливает директорию, где будет запущено пользовательское приложение(если null, то не будет использован)
        /// </summary>
        public string Preparer { get; set; }
        /// <summary>
        /// Черекер (если null либо неверно задан, то стандартный чекер будет использован)
        /// </summary>
        public string Checker { get; set; }
        /// <summary>
        /// Постпроверочные мероприятия(если null, то не будет использован)
        /// </summary>
        public string Finaliter { get; set; }
    }
}