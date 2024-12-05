using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.DataAccess
{
    public class TaskManagementSystemDbContext : DbContext
    {
        public TaskManagementSystemDbContext(DbContextOptions<TaskManagementSystemDbContext> options) : base(options) { }

        public DbSet<Entities.Task> Task { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Task>()
                .Property(t => t.Status)
                .HasConversion<string>();
        }
    }
}
