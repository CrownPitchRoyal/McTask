using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UserManagement.Application.DTO;
using UserManagement.API.IntegrationTests.Helpers;
using UserManagement.Application.DTO.ApiKey;

namespace UserManagement.API.IntegrationTests.Controllers;

public class ApiKeyControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string ApiPath = $"/api/ApiKey";

    public ApiKeyControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    #region AddUser

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsApiKey()
    {
        // Arrange
        var loginDto = new LoginDto 
        {
            userName = "admin",
            password = "TestPass1"
        };

        // Act
        var response = await _client.PostAsJsonAsync(ApiPath, loginDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiKeyDto = await response.Content.ReadFromJsonAsync<ApiKeyDto>();
        Assert.NotNull(apiKeyDto);
        Assert.NotEqual(Guid.Empty, apiKeyDto.Key);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedOrBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto 
        {
            userName = "neObstajam",
            password = "pass"
        };

        // Act
        var response = await _client.PostAsJsonAsync(ApiPath, loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion AddUser

    #region DeleteUser

    [Fact]
    public async Task Logout_WithValidApiKey_ReturnsOk()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Act | Attempt logout
        var response = await _client.DeleteAsync(ApiPath);

        // Assert | Expect an Ok response.
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutApiKey_ReturnsNotFound()
    {
        // Arrange | Ensure no API key header is present.
        _client.DefaultRequestHeaders.Remove("apikey");

        // Act | Attempt logout.
        var response = await _client.DeleteAsync(ApiPath);

        // Assert | Expect a NotFound response when no valid API key is provided.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}