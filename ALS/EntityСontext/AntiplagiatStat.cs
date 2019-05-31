using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.EntityСontext
{
    public class AntiplagiatStat
    {
        public int UserId { get; set; }
        public User User { get; set; }
        [Column(TypeName = "jsonb")]
        public string RequestData { get; set; }
        public DateTime DateCheck { get; set; }
    }
}
