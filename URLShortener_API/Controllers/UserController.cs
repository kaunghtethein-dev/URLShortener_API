using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        // Log in existing user, POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult<DataResult<string>>> LoginUser([FromBody] Dto_LoginUser dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(DataResult<string>.FailResult("Invalid input", StatusCodes.Status400BadRequest));

                var token = await _userService.LoginUserAsync(dto);

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(DataResult<string>.FailResult("Invalid email or password", StatusCodes.Status401Unauthorized));
                }

                return Ok(DataResult<string>.SuccessResult(token, "Login successful", StatusCodes.Status200OK));
            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
           
        }
        // GET: api/user/getuser
        [Authorize]
        [HttpGet("getuser")]
        public async Task<ActionResult<DataResult<Dto_User>>> GetCurrentUser()
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
