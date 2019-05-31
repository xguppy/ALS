using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALS.EntityСontext
{
    public class User
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PwHash { get; set; }
        public string Name {get; set;}
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public int? GroupId { get; set; }
        public List<LaboratoryWork> LaboratoryWorks { get; set; }
        public List<Solution> Solutions { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<AntiplagiatStat> AntiplagiatStats { get; set; }
        public Group Group { get; set; }
    }
}