using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALS.EntityСontext
{
    public class Plan
    {
        public int PlanId { get; set; }
        public int GroupId { get; set; }
        [ForeignKey(nameof(Discipline))]
        [Required]
        public string DisciplineCipher { get; set; }
        public int UserId { get; set; }
        
        public Discipline Discipline { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }
    }
}