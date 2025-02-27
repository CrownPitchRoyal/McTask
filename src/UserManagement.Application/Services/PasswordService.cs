namespace UserManagement.Application.Services;

public class PasswordService
{
    // Password hashing using BCrypt
    public string HashPassword(string password)
    {
        if (!IsValidPassword(password))
        {
            throw new ArgumentException("Password does not meet security requirements.");
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Password verification using BCrypt
    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private static bool IsValidPassword(string password)
    {
        return password.Length >= 8 &&
               password.Any(char.IsDigit) &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower);
    }
}