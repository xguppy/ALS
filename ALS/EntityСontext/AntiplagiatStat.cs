using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class AntiplagiatStat
    {
        public int AntiplagiatStatId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        [Column(TypeName = "jsonb")]
        public string RequestData { get; set; }
        public DateTime DateCheck { get; set; }
    }
}
