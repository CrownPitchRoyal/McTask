using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Attributes;
using UserManagement.Application.DTO;
using UserManagement.Application.Repositories;

namespace UserManagement.API.Controllers;

[ApiKey]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    // GET: api/users/getByName/{username}
    [HttpGet("byName/{username}")]
    public async Task<IActionResult> GetByUserName(string username)
    {
        var user = await userRepository.GetByUserNameAsync(username);
        return user == null ? NotFound() : Ok(user);
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await userRepository.GetAllAsync());
    }

    // POST: api/user
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUserDto addUserDto)
    {
        var createdUser = await userRepository.AddAsync(addUserDto);
        return createdUser != null
            ? CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser)
            : BadRequest();
    }

    // PUT: api/user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        var userDto = await userRepository.UpdateAsync(id, updateUserDto);
        return userDto != null ? Ok(userDto) : NotFound();
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await userRepository.DeleteAsync(id);
        return result ? Ok() : NotFound();
    }
}