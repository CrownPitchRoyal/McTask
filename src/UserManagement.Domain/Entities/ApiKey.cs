namespace UserManagement.Domain.Entities;

public class ApiKey
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Primary key
    public Guid UserId { get; set; } // Foreign key to the User entity
    public Guid Key { get; set; } = Guid.NewGuid(); // API Key
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User User { get; set; }
}