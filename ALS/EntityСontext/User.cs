using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ALS.EntityСontext
{
    public class User: IdentityUser
    {
        public string Name {get; set;}
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public int GroupId { get; set; }
        public List<LaboratoryWork> LaboratoryWorks { get; set; }
        public List<Solution> Solutions { get; set; }
        public Group Group { get; set; }
    }
}