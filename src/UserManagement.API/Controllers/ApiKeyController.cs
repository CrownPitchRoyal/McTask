using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Attributes;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Application.Repositories;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyController(IApiKeyRepository apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var result = await _apiKeyRepository.LoginAsync(loginDto);
        if (!result.Success)
        {
            // Return a 401 Unauthorized with the error message.
            return Unauthorized(new { result.StatusCode, result.Message });
        }
        return Ok(result.ApiKey);
    }

    [ApiKey]
    [HttpDelete]
    public async Task<IActionResult> Logout()
    {
        // Extract the API key from the request headers.
        if (!Request.Headers.TryGetValue("apikey", out var apiKeyValues))
        {
            return NotFound();
        }

        var apiKey = apiKeyValues.First();
        var logoutResult = await _apiKeyRepository.LogoutAsync(apiKey);
        return logoutResult ? Ok() : NotFound();
    }
}