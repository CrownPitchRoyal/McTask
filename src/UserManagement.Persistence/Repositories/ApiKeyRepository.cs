using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Application.Mappers;
using UserManagement.Application.Repositories;
using UserManagement.Application.Services;
using UserManagement.Domain.Entities;
using UserManagement.Persistence.Data;

namespace UserManagement.Persistence.Repositories;

public class ApiKeyRepository(PasswordService passwordService, UserDbContext context) : IApiKeyRepository
{
    // Check if password matches the password that is set to the user
    public async Task<IActionResult> LoginAsync(LoginDto loginDto)
    {
        // Find User
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.userName);
        if (user is null)
            return new UnauthorizedObjectResult(new
            {
                StatusCode = 401,
                Message = "User not found."
            });

        // Compare password to hash-ed one
        if (!passwordService.VerifyHashedPassword(user.PasswordHash, loginDto.password))
            return new UnauthorizedObjectResult(new
            {
                StatusCode = 401,
                Message = "Password is incorrect."
            });

        // Create new Api key entry
        var newApiKey = new ApiKey
        {
            Key = Guid.NewGuid(),
            UserId = user.Id,
            DateCreated = DateTime.UtcNow
        };

        // Add and save
        await context.ApiKeys.AddAsync(newApiKey);
        await context.SaveChangesAsync();

        // Fetch newly created Api key
        var apiKey = await context.ApiKeys.FirstOrDefaultAsync(a => a.Id == newApiKey.Id);

        return new OkObjectResult(apiKey?.ToDto());
    }

    public async Task<bool> LogoutAsync(IHeaderDictionary httpHeaders)
    {
        // Fetch the api key from the headers
        httpHeaders.TryGetValue("apikey", out var foundApiKey);
        var apiKey = foundApiKey.First();

        if (apiKey is null) return false;

        // Parse the Api key
        var parsedApiKey = Guid.Parse(apiKey);

        // Try and find api key to remove
        var apiKeyEntity = await context.ApiKeys.FirstOrDefaultAsync(a => a.Key == parsedApiKey);
        if (apiKeyEntity == null) return false;

        // Save 
        context.ApiKeys.Remove(apiKeyEntity);
        await context.SaveChangesAsync();

        // Remove potential left-over expired Api keys
        await CleanUpExpiredTokens();

        return true;
    }

    // This should be a job.
    private async Task<bool> CleanUpExpiredTokens()
    {
        // Fetch all Api keys with expiration date overdue
        var apiKeys = await context.ApiKeys.Where(a => DateTime.UtcNow > a.DateCreated.AddMinutes(10)).ToListAsync();

        // Batch remove them and save changes
        context.ApiKeys.RemoveRange(apiKeys);
        await context.SaveChangesAsync();
        return true;
    }
}