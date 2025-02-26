using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTO;
using UserManagement.Application.Repositories;
using UserManagement.Domain.Entities;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }
    
    // GET: api/users/getByName/{username}
    [HttpGet("getByName/{username}")]
    public async Task<IActionResult> GetByUserName(string username)
    {
        var user = await _userRepository.GetByUserNameAsync(username);
        return user == null ? NotFound() : Ok(user);
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _userRepository.GetAllAsync());
    }

    // POST: api/user
    [HttpPost]
    public async Task<IActionResult> Add([FromBody]AddUserDto addUserDto)
    {
        var createdUser = await _userRepository.AddAsync(addUserDto);
        return createdUser != null ? CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser) : BadRequest();
    }
    
    // PUT: api/user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody]UpdateUserDto userDto)
    {
        /*if (userDto != null)
        {
        }*/
            return Ok();
    }
    
    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        
        return NotFound();
    }
}