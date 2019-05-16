using System.Collections.Generic;

namespace ALS.EntityСontext
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PwHash { get; set; }
        public string Name {get; set;}
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public int? GroupId { get; set; }
        public List<LaboratoryWork> LaboratoryWorks { get; set; }
        public List<Solution> Solutions { get; set; }
        public Group Group { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}