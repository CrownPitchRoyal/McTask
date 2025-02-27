namespace UserManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; init; } = Guid.NewGuid(); // Immutable ID
        public string UserName { get; set; } = string.Empty; // Default to empty string
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? Language { get; set; }
        public string? Culture { get; set; }
        public string PasswordHash { get; private set; } = string.Empty; // Store hashed password

        // Navigation property | API Keys
        public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();

        // Constructors        
        public User()
        {
        }

        public User(string userName, string? fullName, string email, string? mobileNumber, string? language,
            string? culture)
        {
            this.UserName = userName;
            this.FullName = fullName;
            this.Email = email;
            this.MobileNumber = mobileNumber;
            this.Language = language;
            this.Culture = culture;
        }

        public User(string userName, string? fullName, string email, string? mobileNumber, string? language,
            string? culture, string passwordHash)
        {
            this.UserName = userName;
            this.FullName = fullName;
            this.Email = email;
            this.MobileNumber = mobileNumber;
            this.Language = language;
            this.Culture = culture;
            this.PasswordHash = passwordHash;
        }

        public void SetPassword(string password)
        {
            this.PasswordHash = password;
        }
    }
}