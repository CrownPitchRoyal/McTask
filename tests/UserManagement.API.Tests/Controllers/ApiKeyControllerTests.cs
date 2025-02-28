using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.API.Controllers;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Application.Repositories;

namespace UserManagement.API.Tests.Controllers;

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
        var loginDto = new LoginDto { userName = "admin", password = "TestPass1" };
        var generatedGuid = Guid.NewGuid();
        var expectedResult = new ApiKeyLoginDto()
        {
            Success = true,
            StatusCode = 200,
            ApiKey = new ApiKeyDto { Key = generatedGuid }
        };

        _apiKeyRepositoryMock
            .Setup(repo => repo.LoginAsync(loginDto))
            .ReturnsAsync(expectedResult);
    
        // Act
        var result = await _apiKeyController.Login(loginDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedApiKeyDto = Assert.IsType<ApiKeyDto>(okResult.Value);
        Assert.NotEqual(Guid.Empty, returnedApiKeyDto.Key);
        Assert.Equal(generatedGuid, returnedApiKeyDto.Key);
    }

}