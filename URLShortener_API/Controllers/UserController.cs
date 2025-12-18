using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Shared.DTOs;
using URLShortener_Shared.Wrappers;

namespace URLShortener_API.Controllers
{
    [Route("api/[controller]")]
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DataResult<Dto_User>.FailResult("Invalid input", StatusCodes.Status400BadRequest));
                }


                var createdUser = await _userService.CreateUserAsync(dto_CreateUser);

                if (createdUser == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_User>.FailResult("Failed to create user", StatusCodes.Status500InternalServerError));
                }

                return Ok(DataResult<Dto_User>.SuccessResult(createdUser, "User created successfully", StatusCodes.Status200OK));
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_User>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
           

        }
        // Check if there is already a user with the email
        [HttpGet("checkuserexist/{email}")]
        public async Task<ActionResult> CheckUserAlreadyExists(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(DataResult<string>.FailResult("Invalid input", StatusCodes.Status400BadRequest));
            }
            var userAlreadyExists = await _userService.CheckUserAlreadyExists(email);
            return Ok(DataResult<bool>.SuccessResult(userAlreadyExists,"", StatusCodes.Status200OK));
        }

        // Log in existing user, POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromBody] Dto_LoginUser dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(DataResult<string>.FailResult("Invalid input", StatusCodes.Status400BadRequest));
                }

                var auth = await _userService.LoginUserAsync(dto);

                if (auth == null)
                {
                    return Ok(DataResult<string>.FailResult("Invalid email or password", StatusCodes.Status401Unauthorized));
                }

                return Ok(DataResult<Dto_AuthResponse>.SuccessResult(auth, "Login successful", StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] Dto_RefreshRequest dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                {
                    return BadRequest(DataResult<string>.FailResult("Refresh token required", StatusCodes.Status400BadRequest));
                }

                var auth = await _userService.RefreshTokenAsync(dto.RefreshToken);

                if (auth == null)
                {
                    return Unauthorized(DataResult<string>.FailResult("Invalid or expired refresh token", StatusCodes.Status401Unauthorized));
                }

                return Ok(DataResult<Dto_AuthResponse>.SuccessResult(auth, "Token refreshed", StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
        // GET: api/user/getuser
        [Authorize]
        [HttpGet("getuser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            try
            {
                var email = User.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized(DataResult<Dto_User>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));

                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(DataResult<Dto_User>.FailResult("User not found", StatusCodes.Status404NotFound));

                return Ok(DataResult<Dto_User>.SuccessResult(user, "User retrieved successfully", StatusCodes.Status200OK));
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_User>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
            
        }
    }
}
