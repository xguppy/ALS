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
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<AntiplagiatData> AntiplagiatDatas { get; set; }
        public DbSet<TemplateLaboratoryWork> TemplateLaboratoryWorks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AntiplagiatStat> AntiplagiatStats { get; set; }


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public ApplicationContext()
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ForNpgsqlHasEnum<Evaluation>();
            modelBuilder.ForNpgsqlHasEnum<RoleEnum>();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Group>()
                .HasIndex(g => new {g.Name, g.Year})
                .IsUnique();
            modelBuilder.Entity<Variant>()
                .HasIndex(var => new {var.VariantNumber, var.LaboratoryWorkId})
                .IsUnique();
            modelBuilder.Entity<AntiplagiatData>()
                .HasIndex(antiplagiatData => new {antiplagiatData.SolutionFirstId, antiplagiatData.SolutionSecondId})
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=als;Username=postgres;Password=postgres");
    }
}