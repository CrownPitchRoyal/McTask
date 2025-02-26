using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;


namespace UserManagement.Persistence.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
