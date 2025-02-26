﻿using UserManagement.Application.DTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto?> GetByUserNameAsync(string userName);
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> AddAsync(AddUserDto addUserDto);
        Task UpdateAsync(UpdateUserDto updateUserDto);
        Task DeleteAsync(Guid id);
    }
}
