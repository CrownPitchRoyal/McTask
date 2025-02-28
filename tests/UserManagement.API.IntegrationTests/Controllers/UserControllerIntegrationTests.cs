using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UserManagement.Application.DTO;
using UserManagement.API.IntegrationTests.Helpers;

namespace UserManagement.API.IntegrationTests.Controllers;

public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string ApiPath = $"/api/User";

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUserById_ReturnsOkResult_WithUser()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsnyc("admin", "TestPass1");
        
        // Arrange | Add a user with post
        var addUserDto = new AddUserDto
        {
            UserName = "IntegrationTestUser",
            FullName = "Integration Test User",
            Email = "IntegrationTestUser@local.com",
            MobileNumber = "+38623123123",
            Language = "sl-SI",
            Culture = "en-gb",
            Password = "IntegrationTestPassword12"
        };

        // Post new user
        var postResponse = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        postResponse.EnsureSuccessStatusCode();
        
        // Retrieve new user
        var createdUser = await postResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(createdUser);
        
        // Act
        var response = await _client.GetAsync(ApiPath + $"{createdUser.Id}");
        response.EnsureSuccessStatusCode();
        
        var retrievedUser = await response.Content.ReadFromJsonAsync<UserDto>();
        
        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(createdUser.Id, retrievedUser.Id);
        Assert.Equal(createdUser.UserName, retrievedUser.UserName);
        Assert.Equal(createdUser.FullName, retrievedUser.FullName);
        Assert.Equal(createdUser.Email, retrievedUser.Email);
        Assert.Equal(createdUser.MobileNumber, retrievedUser.MobileNumber);
        Assert.Equal(createdUser.Language, retrievedUser.Language);
        Assert.Equal(createdUser.Culture, retrievedUser.Culture);
    }
    
    [Fact]
    public async Task GetUserById_ReturnsNotFoundResult_ForNonExistingUser()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsnyc("admin", "TestPass1");
        
        // Arrange
        var nonExistingUserId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync(ApiPath + $"{nonExistingUserId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddUser_Then_GetUserById_ReturnsOk()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsnyc("admin", "TestPass1");
        
        // Arrange
        var addUserDto = new AddUserDto
        {
            UserName = "IntegrationTestUser",
            FullName = "Integration Test User",
            Email = "IntegrationTestUser@local.com",
            MobileNumber = "+38623123123",
            Language = "sl-SI",
            Culture = "en-gb",
            Password = "IntegrationTestPassword12"
        };

        // Act - Add a new user
        var postResponse = await _client.PostAsJsonAsync("api/user", addUserDto);
        postResponse.EnsureSuccessStatusCode();
        var createdUser = await postResponse.Content.ReadFromJsonAsync<UserDto>();

        // Act - Retrieve the same user by id
        var getResponse = await _client.GetAsync($"api/user/{createdUser.Id}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.NotNull(retrievedUser.Id);
        Assert.Equal(createdUser.UserName, retrievedUser.UserName);
        Assert.Equal(createdUser.FullName, retrievedUser.FullName);
        Assert.Equal(createdUser.Email, retrievedUser.Email);
        Assert.Equal(createdUser.MobileNumber, retrievedUser.MobileNumber);
        Assert.Equal(createdUser.Language, retrievedUser.Language);
        Assert.Equal(createdUser.Culture, retrievedUser.Culture);
    }
}


