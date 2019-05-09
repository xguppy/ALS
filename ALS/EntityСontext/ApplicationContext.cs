using Microsoft.EntityFrameworkCore;

namespace ALS.EntityСontext
{
    public class ApplicationContext: DbContext
    {
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<LaboratoryWork> LaboratoryWorks { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<Variant> Variants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ForNpgsqlHasEnum<Evaluation>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=als;Username=postgres;Password=postgres");
    }
}