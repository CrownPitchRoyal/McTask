using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTO;
using UserManagement.Application.Mappers;
using UserManagement.Application.Repositories;
using UserManagement.Domain.Entities;
using UserManagement.Persistence.Data;


namespace UserManagement.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user.ToDto();
        }


        public async Task<UserDto?> GetByUserNameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            return user.ToDto();
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
            _context.Users.Add(user); // Add to table
            
            await _context.SaveChangesAsync(); //save
            
            // Get created user | verify it was created
            return await GetByIdAsync(user.Id);
        }

        public async Task UpdateAsync(UpdateUserDto updateUserDto)
        {
            var user = updateUserDto.ToEntity();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is not null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
