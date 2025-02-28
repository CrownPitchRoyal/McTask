using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Application.Mappers;
using UserManagement.Application.Repositories;
using UserManagement.Application.Services;
using UserManagement.Domain.Entities;
using UserManagement.Persistence.Data;

namespace UserManagement.Persistence.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly PasswordService _passwordService;
    private readonly UserDbContext _context;

    public ApiKeyRepository(PasswordService passwordService, UserDbContext context)
    {
        _passwordService = passwordService;
        _context = context;
    }

    public async Task<ApiKeyLoginDto> LoginAsync(LoginDto loginDto)
    {
        // Find the user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.userName);
        if (user is null)
        {
            return new ApiKeyLoginDto 
            { 
                Success = false, 
                StatusCode = 401, 
                Message = "User not found." 
            };
        }

        // Compare the provided password with the stored hashed password
        if (!_passwordService.VerifyHashedPassword(user.PasswordHash, loginDto.password))
        {
            return new ApiKeyLoginDto 
            { 
                Success = false, 
                StatusCode = 401, 
                Message = "Password is incorrect." 
            };
        }

        // Create a new API key entry
        var newApiKey = new ApiKey
        {
            Key = Guid.NewGuid(),
            UserId = user.Id,
            DateCreated = DateTime.UtcNow
        };

        await _context.ApiKeys.AddAsync(newApiKey);
        await _context.SaveChangesAsync();

        // Fetch the newly created API key and map to DTO
        var apiKey = await _context.ApiKeys.FirstOrDefaultAsync(a => a.Id == newApiKey.Id);

        return new ApiKeyLoginDto 
        { 
            Success = true, 
            StatusCode = 200, 
            ApiKey = apiKey?.ToDto() 
        };
    }

    public async Task<bool> LogoutAsync(string apiKeyString)
    {
        if (string.IsNullOrWhiteSpace(apiKeyString))
            return false;

        if (!Guid.TryParse(apiKeyString, out Guid parsedApiKey))
            return false;

        // Try to find the API key entry
        var apiKeyEntity = await _context.ApiKeys.FirstOrDefaultAsync(a => a.Key == parsedApiKey);
        if (apiKeyEntity == null)
            return false;

        _context.ApiKeys.Remove(apiKeyEntity);
        await _context.SaveChangesAsync();

        // Remove potential expired API keys
        await CleanUpExpiredTokens();
        return true;
    }

    // This method removes API keys older than 10 minutes.
    private async Task<bool> CleanUpExpiredTokens()
    {
        var expiredApiKeys = await _context.ApiKeys
            .Where(a => DateTime.UtcNow > a.DateCreated.AddMinutes(10))
            .ToListAsync();

        _context.ApiKeys.RemoveRange(expiredApiKeys);
        await _context.SaveChangesAsync();
        return true;
    }
}
