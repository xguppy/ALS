using System.ComponentModel.DataAnnotations.Schema;
using ALS.EntityСontext;

namespace ALS.DTO
{
    public class LaboratoryWorkDTO
    {
        public int TemplateLaboratoryWorkId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Evaluation Evaluation { get; set; }
        public string Cipher { get; set; }
        public int UserId { get; set; }
        public string Constraints { get; set; }
    }
}