using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Attributes;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Application.Repositories;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeyController(IApiKeyRepository apiKeyRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        return await apiKeyRepository.LoginAsync(loginDto);
    }

    // We pass headers so the api key can be extracted and searched for in the db
    [ApiKey]
    [HttpDelete]
    public async Task<IActionResult> Logout()
    {
        var result = await apiKeyRepository.LogoutAsync(this.HttpContext.Request.Headers);
        return result ? Ok() : NotFound();
    }
}