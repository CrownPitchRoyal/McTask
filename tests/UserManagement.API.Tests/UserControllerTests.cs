using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.API.Controllers;
using UserManagement.Application.DTO;
using UserManagement.Application.Repositories;

namespace UserManagement.API.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserController _userController;
    
    public UserControllerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userController = new UserController(_userRepositoryMock.Object);
    }
    
    #region GetUserById

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto { Id = userId, UserName = "Testn" };
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(userDto);

        // Act
        var result = await _userController.GetById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userId, returnedUser.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((UserDto)null);

        // Act
        var result = await _userController.GetById(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion GetUserById

    #region GetByUserName
    
    [Fact]
    public async Task GetByUserName_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var userName = "Testn";
        var userDto = new UserDto { Id = Guid.NewGuid(), UserName = userName };
        _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userName)).ReturnsAsync(userDto);

        // Act
        var result = await _userController.GetByUserName(userName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userName, returnedUser.UserName);
    }

    [Fact]
    public async Task GetByUserName_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        var userName = "IDontExist";
        _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(userName)).ReturnsAsync((UserDto)null);

        // Act
        var result = await _userController.GetByUserName(userName);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    #endregion GetByUserName
    
    #region GetAllUsers

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = Guid.NewGuid(), UserName = "User1" },
            new UserDto { Id = Guid.NewGuid(), UserName = "User2" }
        };
        _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Check if result is OkObjectResult
        var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value); // Check if the result contains a list of UserDto
        Assert.Equal(users.Count, returnedUsers.Count); // Verify the count of returned users matches
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var users = new List<UserDto>(); // Empty list of users
        _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Check if result is OkObjectResult
        var returnedUsers = Assert.IsType<List<UserDto>>(okResult.Value); // Check if the result contains a list of UserDto
        Assert.Empty(returnedUsers); // Verify the list is empty
    }

    #endregion GetAllUsers
    
    #region AddUser

    [Fact]
    public async Task Add_ReturnsCreatedAtAction_WhenUserIsCreated()
    {
        // Arrange
        var addUserDto = new AddUserDto { UserName = "TestnUser", FullName = "Testn User", Email = "testn@domenca.com" };
        var createdUser = new UserDto { Id = Guid.NewGuid(), UserName = "TestnUser" };
        _userRepositoryMock.Setup(repo => repo.AddAsync(addUserDto)).ReturnsAsync(createdUser);

        // Act
        var result = await _userController.Add(addUserDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedUser = Assert.IsType<UserDto>(createdAtActionResult.Value);
        Assert.Equal(createdUser.Id, returnedUser.Id);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenUserCannotBeCreated()
    {
        // Arrange
        var addUserDto = new AddUserDto { UserName = "TestnUser", FullName = "Testn User", Email = "testn@domenca.com" };
        _userRepositoryMock.Setup(repo => repo.AddAsync(addUserDto)).ReturnsAsync((UserDto)null);

        // Act
        var result = await _userController.Add(addUserDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    #endregion AddUser

    #region UpdateUser

    [Fact]
    public async Task Update_ReturnsOkResult_WhenUserIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto { UserName = "UpdatedUser", FullName = "Updated User" };
        var updatedUser = new UserDto { Id = userId, UserName = "UpdatedUser" };
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(userId, updateUserDto)).ReturnsAsync(updatedUser);

        // Act
        var result = await _userController.Update(userId, updateUserDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(updatedUser.Id, returnedUser.Id);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto { UserName = "UpdatedUser", FullName = "Updated User" };
        _userRepositoryMock.Setup(repo => repo.UpdateAsync(userId, updateUserDto)).ReturnsAsync((UserDto)null);

        // Act
        var result = await _userController.Update(userId, updateUserDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion UpdateUser

    #region DeleteUser

    [Fact]
    public async Task Delete_ReturnsOkResult_WhenUserIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.DeleteAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _userController.Delete(userId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.DeleteAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _userController.Delete(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion DeleteUser
}