using Microsoft.EntityFrameworkCore;
using MyTestProject.Models;

namespace MyTestProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().HasMany(e => e.Shifts).WithOne(s => s.Employee).HasForeignKey(s => s.EmployeeId);
        }
    }
}
