using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class GenExtension
    {
        public int GenExtensionId { get; set; }
        [Required]
        public string Extension { get; set; }
    }
}
