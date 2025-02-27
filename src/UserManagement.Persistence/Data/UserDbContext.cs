using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Persistence.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<ApiKey>()
                .HasOne(u => u.User)
                .WithMany(u => u.ApiKeys)
                .HasForeignKey(u => u.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}