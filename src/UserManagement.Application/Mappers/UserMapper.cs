using UserManagement.Application.DTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        return new UserDto(
            user.Id,
            user.UserName,
            user.FullName,
            user.Email,
            user.MobileNumber,
            user.Language,
            user.Culture
        );
    }
    
    public static User ToDto(this UserDto userDto)
    {
        if (userDto == null)
            throw new ArgumentNullException(nameof(userDto), "UserDto cannot be null");
        return new User(
            userDto.UserName,
            userDto.FullName,
            userDto.Email,
            userDto.MobileNumber,
            userDto.Language,
            userDto.Culture
        );
    }
    
    public static User ToEntity(this AddUserDto addUserDto)
    {
        if (addUserDto == null)
            throw new ArgumentNullException(nameof(addUserDto), "AddUserDto cannot be null");
        return new User(
            addUserDto.UserName,
            addUserDto.FullName,
            addUserDto.Email,
            addUserDto.MobileNumber,
            addUserDto.Language,
            addUserDto.Culture,
            addUserDto.Password
        );
    }
    
    public static User ToEntity(this UpdateUserDto createUserDto)
    {
        if (createUserDto == null)
            throw new ArgumentNullException(nameof(createUserDto), "UpdateUserDto cannot be null");
        return new User(
            createUserDto.UserName,
            createUserDto.FullName,
            createUserDto.Email,
            createUserDto.MobileNumber,
            createUserDto.Language,
            createUserDto.Culture,
            createUserDto.Password
        );
    }
}