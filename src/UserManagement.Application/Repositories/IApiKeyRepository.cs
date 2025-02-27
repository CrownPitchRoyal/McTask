using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTO.ApiKey;

namespace UserManagement.Application.Repositories;

public interface IApiKeyRepository
{
    Task<IActionResult> LoginAsync(LoginDto loginDto);
    Task<bool> LogoutAsync(IHeaderDictionary httpHeaders);
}