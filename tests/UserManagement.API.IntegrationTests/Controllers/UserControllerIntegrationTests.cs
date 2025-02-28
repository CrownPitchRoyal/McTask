using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UserManagement.Application.DTO;
using UserManagement.API.IntegrationTests.Helpers;
using UserManagement.Application.DTO.ApiKey;

namespace UserManagement.API.IntegrationTests.Controllers;

public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string ApiPath = $"/api/User";

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region GetUserById

    [Fact]
    public async Task GetUserById_ReturnsOkResult_WithUser()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

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
        var response = await _client.GetAsync($"{ApiPath}/{createdUser.Id}");
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
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Arrange | Get random Guid for non-existing user
        var nonExistingUserId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"{ApiPath}/{nonExistingUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region GetByUserName

    [Fact]
    public async Task GetByUserName_ReturnsOkResult_WhenUserExists()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        var addUserDto = new AddUserDto
        {
            UserName = "UserByNameTest",
            FullName = "User ByName Test",
            Email = "userbyname@local.com",
            MobileNumber = "123456789",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "TestPassword123!"
        };

        // Act | Add new user
        var postResponse = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        postResponse.EnsureSuccessStatusCode();
        var createdUser = await postResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(createdUser);

        // Act | Retrieve user by username using GET /api/User/byName/{username}
        var getResponse = await _client.GetAsync($"{ApiPath}/byName/{createdUser.UserName}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();

        // Assert | Verify that the retrieved user matches the created one
        Assert.NotNull(retrievedUser);
        Assert.Equal(createdUser.Id, retrievedUser.Id);
        Assert.Equal(createdUser.UserName, retrievedUser.UserName);
    }

    [Fact]
    public async Task GetByUserName_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Act | Attempt to retrieve a user with a non-existing username
        var getResponse = await _client.GetAsync($"{ApiPath}/byName/qawesrdtfzguhijokplš34x5c64v7b");

        // Assert | Expect a NotFound response
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    #endregion

    #region GetAll

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Create first user
        var addUserDto1 = new AddUserDto
        {
            UserName = "AllUserTest1",
            FullName = "User All Test 1",
            Email = "alltest1@local.com",
            MobileNumber = "1111111111",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "Password123"
        };
        var postResponse1 = await _client.PostAsJsonAsync(ApiPath, addUserDto1);
        postResponse1.EnsureSuccessStatusCode();

        // Create second user
        var addUserDto2 = new AddUserDto
        {
            UserName = "AllUserTest2",
            FullName = "User All Test 2",
            Email = "alltest2@local.com",
            MobileNumber = "+2222222222",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "Password456"
        };
        var postResponse2 = await _client.PostAsJsonAsync(ApiPath, addUserDto2);
        postResponse2.EnsureSuccessStatusCode();

        // Act | Retrieve all users using GET /api/User
        var getResponse = await _client.GetAsync(ApiPath);
        getResponse.EnsureSuccessStatusCode();
        var users = await getResponse.Content.ReadFromJsonAsync<List<UserDto>>();

        // Assert | Ensure the returned list is not null and contains at least the two newly added users.
        Assert.NotNull(users);
        Assert.True(users.Count >= 2);
    }

    #endregion

    #region AddUser

    [Fact]
    public async Task AddUser_Then_GetUserById_ReturnsOk()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

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
        var postResponse = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        postResponse.EnsureSuccessStatusCode();
        var createdUser = await postResponse.Content.ReadFromJsonAsync<UserDto>();

        // Act - Retrieve the same user by id
        var getResponse = await _client.GetAsync($"{ApiPath}/{createdUser!.Id}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.NotEqual(Guid.Empty, retrievedUser.Id);
        Assert.Equal(createdUser!.UserName, retrievedUser.UserName);
        Assert.Equal(createdUser.FullName, retrievedUser.FullName);
        Assert.Equal(createdUser.Email, retrievedUser.Email);
        Assert.Equal(createdUser.MobileNumber, retrievedUser.MobileNumber);
        Assert.Equal(createdUser.Language, retrievedUser.Language);
        Assert.Equal(createdUser.Culture, retrievedUser.Culture);
    }

    [Fact]
    public async Task AddUser_ReturnsCreatedAtAction_WithLocationHeader()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        var addUserDto = new AddUserDto
        {
            UserName = "ITU12",
            FullName = "ITU",
            Email = "ITU@local.com",
            MobileNumber = "+38623123123",
            Language = "sl-SI",
            Culture = "en-gb",
            Password = "IntegrationTestPassword12"
        };

        // Act
        var postResponse = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        postResponse.EnsureSuccessStatusCode();

        // Assert | Verify status and Location header
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenInvalidData()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Arrange | Create an invalid AddUserDto. Missing UserName, password shouldn't pass validation
        var invalidUserDto = new AddUserDto
        {
            // UserName intentionally left null to simulate invalid data
            FullName = "No Username",
            Email = "nouser@local.com",
            MobileNumber = "22222222",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "WeakPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync(ApiPath, invalidUserDto);

        // Assert | Expect a BadRequest response
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_WhenUserAlreadyExists()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        var addUserDto = new AddUserDto
        {
            UserName = "DuplicateUser",
            FullName = "Duplicate User",
            Email = "duplicate@local.com",
            MobileNumber = "3333333",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "ValidPassword123"
        };

        // Act | Create user the first time
        var response1 = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        response1.EnsureSuccessStatusCode();

        // Act | Try to create the same user again
        var response2 = await _client.PostAsJsonAsync(ApiPath, addUserDto);

        // Assert | Expect BadRequest for duplicate user creation
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    #endregion AddUser

    #region UpdateUser

    [Fact]
    public async Task UpdateUser_ReturnsOkResult_WhenUserIsUpdated()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Arrange | Create a new user first
        var addUserDto = new AddUserDto
        {
            UserName = "UpdateUserTest123",
            FullName = "Update User Test",
            Email = "updateusertest@local.com",
            MobileNumber = "00000000",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "Password1!"
        };

        var postResponse = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        postResponse.EnsureSuccessStatusCode();
        var createdUser = await postResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(createdUser);

        // Act | Update the user using PUT. Prepare update data.
        var updateUserDto = new UpdateUserDto
        {
            // Assuming these are the updatable fields:
            UserName = "123UpdateUserTest123",
            FullName = "Updated User Test",
            Email = "brap@local.com",
            MobileNumber = "555555",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "BrapBrap!"
        };

        var putResponse = await _client.PutAsJsonAsync($"{ApiPath}/{createdUser.Id}", updateUserDto);
        putResponse.EnsureSuccessStatusCode();
        var updatedUser = await putResponse.Content.ReadFromJsonAsync<UserDto>();

        // Assert | Check that updated fields reflect new values
        Assert.NotNull(updatedUser);
        Assert.Equal(createdUser.Id, updatedUser.Id);
        Assert.Equal(updateUserDto.UserName, updatedUser.UserName);
        Assert.Equal(updateUserDto.FullName, updatedUser.FullName);
        Assert.Equal(updateUserDto.Email, updatedUser.Email);
        Assert.Equal(updateUserDto.MobileNumber, updatedUser.MobileNumber);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Arrange | Create update data for a non-existing user
        var updateUserDto = new UpdateUserDto
        {
            UserName = "123456789adsfghjk",
            FullName = "Non Existing User"
        };

        // Act | Attempt update with a random Guid
        var nonExistingUserId = Guid.NewGuid();
        var putResponse = await _client.PutAsJsonAsync($"{ApiPath}/{nonExistingUserId}", updateUserDto);

        // Assert | Expect NotFound status code
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
    }

    #endregion

    #region DeleteUser

    [Fact]
    public async Task DeleteUser_ReturnsOkResult_WhenUserIsDeleted()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Arrange | Create a new user first
        var addUserDto = new AddUserDto
        {
            UserName = "DeleteMe",
            FullName = "Delete me",
            Email = "deletus@local.com",
            MobileNumber = "0000000",
            Language = "sl-SI",
            Culture = "sl-SI",
            Password = "DeleteMe!"
        };

        var postResponse = await _client.PostAsJsonAsync(ApiPath, addUserDto);
        postResponse.EnsureSuccessStatusCode();
        var createdUser = await postResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(createdUser);

        // Act | Delete the user
        var deleteResponse = await _client.DeleteAsync($"{ApiPath}/{createdUser.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Assert | Attempt to retrieve the deleted user and expect NotFound
        var getResponse = await _client.GetAsync($"{ApiPath}/{createdUser.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange | Get api key via auth helper method
        await _client.AuthenticateAsync("admin", "TestPass1");

        // Arrange | Use a random Guid for non-existing user
        var nonExistingUserId = Guid.NewGuid();

        // Act | Attempt to delete
        var deleteResponse = await _client.DeleteAsync($"{ApiPath}/{nonExistingUserId}");

        // Assert | Expect a NotFound response
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    #endregion
}