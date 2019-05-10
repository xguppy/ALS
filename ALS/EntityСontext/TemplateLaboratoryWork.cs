using System.Collections.Generic;

namespace ALS.EntityСontext
{
    public class TemplateLaboratoryWork
    {
        public int TemplateLaboratoryWorkId { get; set; }
        public string TemplateTask { get; set; }
        public List<LaboratoryWork> LaboratoryWorks { get; set; }
    }
}