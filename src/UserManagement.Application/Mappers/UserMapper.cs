using UserManagement.Application.DTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
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