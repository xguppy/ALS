using System.Collections.Generic;

namespace ALS.EntityСontext
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string SpecialityId { get; set; }
        public List<User> Users { get; set; }
        public Specialty Specialty { get; set; }
    }
}