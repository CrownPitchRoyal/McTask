using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.API.Controllers;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Application.Repositories;

namespace UserManagement.API.Tests;

public class ApiKeyControllerTests
{
    private readonly Mock<IApiKeyRepository> _apiKeyRepositoryMock;
    private readonly ApiKeyController _apiKeyController;
    
    public ApiKeyControllerTests()
    {
        _apiKeyRepositoryMock = new Mock<IApiKeyRepository>();
        _apiKeyController = new ApiKeyController(_apiKeyRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Login_ReturnsOkResult_WithApiKey()
    {
        // Arrange
        var loginDto = new LoginDto() { userName = "admin", password = "TestPass1" };
        var apiKey = Guid.NewGuid(); // Simulate an approximation of a generated API key
        _apiKeyRepositoryMock.Setup(repo => repo.LoginAsync(loginDto))
            .ReturnsAsync(new OkObjectResult(apiKey.ToString()));
        
        // Act
        var result = await _apiKeyController.Login(loginDto);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Check result type
        var returnedApiKey = Assert.IsType<string>(okResult.Value); // Check value type
        Assert.NotNull(returnedApiKey); // Make sure it's not null
        Assert.True(Guid.TryParse(returnedApiKey, out _)); // Validate, to make sure it's a valid GUID
    }
}