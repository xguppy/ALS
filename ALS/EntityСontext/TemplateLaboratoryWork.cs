using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class TemplateLaboratoryWork
    {
        public int TemplateLaboratoryWorkId { get; set; }
        [Required]
        public int ThemeId { get; set; }
        [Required]
        public string TemplateTask { get; set; }

        public Theme Theme { get; set; }
        public List<LaboratoryWork> LaboratoryWorks { get; set; }
    }
}