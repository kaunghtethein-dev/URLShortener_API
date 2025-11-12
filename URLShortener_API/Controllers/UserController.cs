using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Shared.DTOs;
using URLShortener_Shared.Wrappers;

namespace URLShortener_API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Create (Register) a new user, POST: api/user/createuser
        [HttpPost("createuser")]
        public async Task<ActionResult> CreateUser([FromBody] Dto_CreateUser dto_CreateUser)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(DataResult<Dto_User>.FailResult("Invalid input", 400));
            }

                
            var createdUser = await _userService.CreateUserAsync(dto_CreateUser);

            if (createdUser == null)
            {
                return StatusCode(500, DataResult<Dto_User>.FailResult("Failed to create user", 500));
            }
                
            return Ok(DataResult<Dto_User>.SuccessResult(createdUser, "User created successfully", 201));

        }

        // Log in existing user, POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult<DataResult<string>>> LoginUser([FromBody] Dto_LoginUser dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(DataResult<string>.FailResult("Invalid input", 400));

            var token = await _userService.LoginUserAsync(dto);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(DataResult<string>.FailResult("Invalid email or password", 401));
            }
                

            return Ok(DataResult<string>.SuccessResult(token, "Login successful", 200));
        }
        // GET: api/user/getuser
        [Authorize]
        [HttpGet("getuser")]
        public async Task<ActionResult<DataResult<Dto_User>>> GetCurrentUser()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized(DataResult<Dto_User>.FailResult("Unauthorized", 401));

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound(DataResult<Dto_User>.FailResult("User not found", 404));

            return Ok(DataResult<Dto_User>.SuccessResult(user, "User retrieved successfully", 200));
        }
    }
}
