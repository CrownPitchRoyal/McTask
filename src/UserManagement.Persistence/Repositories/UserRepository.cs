using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTO;
using UserManagement.Application.Mappers;
using UserManagement.Application.Repositories;
using UserManagement.Application.Services;
using UserManagement.Domain.Entities;
using UserManagement.Persistence.Data;


namespace UserManagement.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private PasswordService _passwordService;

        public UserRepository(UserDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user?.ToDto();
        }


        public async Task<UserDto?> GetByUserNameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            return user?.ToDto();
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync(); // Get All Users
            var usersDto = users.Select(user => user.ToDto()).ToList(); // Map Entity to UserDto

            return usersDto;
        }

        public async Task<UserDto?> AddAsync(AddUserDto addUserDto)
        { 
            var user = addUserDto.ToEntity(); // Map to entity
            
            var doesUserExist = await _context.Users.AnyAsync(u => u.UserName == addUserDto.UserName);
            if (doesUserExist) return null; // Username taken!
            
            user.SetPassword(_passwordService.HashPassword(addUserDto.Password));
            
            _context.Users.Add(user); // Add to table
            
            await _context.SaveChangesAsync(); //save
            
            // Get created user | verify it was created
            return await GetByIdAsync(user.Id);
        }

        public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id); // Fetch user
            if (user == null) return null;
            
            // Update user fields
            user.UserName = updateUserDto.UserName;
            user.FullName = updateUserDto.FullName ?? user.FullName;
            user.Email = updateUserDto.Email;
            user.MobileNumber = updateUserDto.MobileNumber ?? user.MobileNumber;
            user.Language = updateUserDto.Language ?? user.Language;
            user.Culture = updateUserDto.Culture ?? user.Culture;
            if(updateUserDto.Password != "")
                user.SetPassword(_passwordService.HashPassword(updateUserDto.Password));
            
            // Save changes
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            
            // Return updated user
            return await GetByIdAsync(user.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
