namespace UserManagement.Application.DTO;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty; // Default to empty string
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Language { get; set; }
    public string? Culture { get; set; }

    public UserDto()
    {
    }

    public UserDto(Guid id, string userName, string? fullName, string email, string? mobileNumber, string? language,
        string? culture)
    {
        this.Id = id;
        this.UserName = userName;
        this.FullName = fullName;
        this.Email = email;
        this.MobileNumber = mobileNumber;
        this.Language = language;
        this.Culture = culture;
    }
}