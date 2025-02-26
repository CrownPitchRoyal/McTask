using UserManagement.Domain.Entities;
using UserManagement.Persistence.Data;

namespace UserManagement.Persistence;

public static class UserDbContextSeed
{
    public static async Task SeedAsync(UserDbContext context)
    {
        if (!context.Users.Any()) // Make sure to only add users if table empty
        {
            context.Users.AddRange(new List<User>
            {
                new User ("admin", "Admin", "admin@example.com", "123456789", "en", "en-US", "admin"),
                new User ("test", "Test User", "test@example.com", "123123123", "en",  "en-US", "test")
            });
            
            await context.SaveChangesAsync();
        }
    }
}