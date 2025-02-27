namespace UserManagement.Application.DTO;

public class AddUserDto
{
    public string UserName { get; set; } = string.Empty; // Default to empty string
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Language { get; set; }
    public string? Culture { get; set; }
    public string Password { get; set; } = string.Empty;
}