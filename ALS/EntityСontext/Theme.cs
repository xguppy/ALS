using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class Theme
    {
        public int ThemeId { get; set; }
        [Required]
        public string DisciplineCipher { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Name { get; set; }

        public Discipline Discipline { get; set; }

        public List<LaboratoryWork> LaboratoryWorks { get; set; }
        public List<TemplateLaboratoryWork> TemplateLaboratoryWorks { get; set; }

    }
}
