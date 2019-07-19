using System;

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
        public int? SolutionId { get; set; }
        public string SourceCode { get; set; }
        /// <summary>
        /// Should system check works of user that work is checking??
        /// </summary>
        public bool CheckUserWork { get; set; } = false;
        public bool CheckTime { get; set; } = false;

        /// <summary>
        /// Period of Date for check (first date)
        /// </summary>
        public DateTime? DateTimeFirst { get; set; }
        /// <summary>
        /// Period of Date for check (last date)
        /// </summary>
        public DateTime? DateTimeLast { get; set; }
    }
}
