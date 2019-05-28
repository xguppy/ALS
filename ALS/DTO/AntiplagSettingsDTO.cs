using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.DTO
{
    /// <summary>
    /// Settings for checking works
    /// </summary>
    public class AntiplagSettingsDTO
    {
        /// <summary>
        /// Count of most common results
        /// </summary>
        public int CountResults { get; set; }
        /// <summary>
        /// id of checked work
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Should system check works of user that work is checking??
        /// </summary>
        public bool CheckUserWork { get; set; } = false;

        /// <summary>
        /// Cryterias for sorting
        /// </summary>
        public SortBy[] SortCryterias { get; set; } = new SortBy[] { };

        /// <summary>
        /// Period of Date for check (first date)
        /// </summary>
        public DateTime? DateTimeFirst { get; set; }
        /// <summary>
        /// Period of Date for check (last date)
        /// </summary>
        public DateTime? DateTimeLast { get; set; }

        /// <summary>
        /// Groups to sort
        /// </summary>
        public int[] GroupIds { get; set; }
        /// <summary>
        /// Disciplines to sort
        /// </summary>
        public int[] DisciplineIds { get; set; }
    }

    public enum SortBy
    {
        Discipline,
        Group,
        Time
    }
}
