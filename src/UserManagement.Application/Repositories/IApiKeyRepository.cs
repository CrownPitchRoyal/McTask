using UserManagement.Application.DTO.ApiKey;

namespace UserManagement.Application.Repositories;

public interface IApiKeyRepository
{
    Task<ApiKeyLoginDto> LoginAsync(LoginDto loginDto);
    Task<bool> LogoutAsync(string apiKeyString);
}