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

        public User()
        {
        }

        public User(string userName, string? fullName, string email, string? mobileNumber, string? language, string? culture)
        {
            this.UserName = userName;
            this.FullName = fullName;
            this.Email = email;
            this.MobileNumber = mobileNumber;
            this.Language = language;
            this.Culture = culture;
        }
        
        public User(string userName, string? fullName, string email, string? mobileNumber, string? language, string? culture, string password)
        {
            this.UserName = userName;
            this.FullName = fullName;
            this.Email = email;
            this.MobileNumber = mobileNumber;
            this.Language = language;
            this.Culture = culture;
            SetPassword(password);
        }

        // Method to hash the password
        public void SetPassword(string password)
        {
            PasswordHash = HashPassword(password);
        }

        // Method to verify the password
        public bool VerifyPassword(string password)
        {
            return VerifyHashedPassword(PasswordHash, password);
        }

        // Password hashing using BCrypt
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Password verification using BCrypt
        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
