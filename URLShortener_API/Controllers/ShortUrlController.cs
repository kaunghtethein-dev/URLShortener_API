using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Shared.DTOs;
using URLShortener_Shared.Wrappers;

namespace URLShortener_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public ShortUrlController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }
        [HttpPost("create")]
        public async Task<ActionResult<DataResult<Dto_ShortUrl>>> CreateShortUrl([FromBody] Dto_CreateShortUrl dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(DataResult<Dto_ShortUrl>.FailResult("Invalid input", StatusCodes.Status400BadRequest));

                var created = await _shortUrlService.CreateShortUrlAsync(dto);

                if (created == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_ShortUrl>.FailResult("Failed to create short URL", StatusCodes.Status500InternalServerError));
                }
                    

                return Ok(DataResult<Dto_ShortUrl>.SuccessResult(created, "Short URL created successfully", StatusCodes.Status200OK));
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_ShortUrl>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
           
        }
        [Authorize]
        [HttpGet("getbyuser")]
        public async Task<ActionResult<DataResult<IEnumerable<Dto_ShortUrl>>>> GetUserShortUrls()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<IEnumerable<Dto_ShortUrl>>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }
                    
                int userId = int.Parse(userIdClaim);
                var urls = await _shortUrlService.GetByUserIdAsync(userId);

                return Ok(DataResult<IEnumerable<Dto_ShortUrl>>.SuccessResult(urls, "Success", StatusCodes.Status200OK));
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<IEnumerable<Dto_ShortUrl>>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
          
        }

        [Authorize]
        [HttpGet("{id:long}")]
        public async Task<ActionResult<DataResult<Dto_ShortUrl>>> GetById(long id)
        {
            try
            {
                var dto = await _shortUrlService.GetByIdAsync(id);
                if (dto == null)
                {
                    return NotFound(DataResult<Dto_ShortUrl>.FailResult("Not found", StatusCodes.Status404NotFound));
                }

                // Only owner can view details
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<Dto_ShortUrl>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }
                    

                int userId = int.Parse(userIdClaim);
                if (dto.UserId == null || dto.UserId != userId)
                {
                    return Forbid();
                }
                   

                return Ok(DataResult<Dto_ShortUrl>.SuccessResult(dto, "Success", StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_ShortUrl>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }

        [Authorize]
        [HttpPut("update/{id:long}")]
        public async Task<ActionResult<DataResult<Dto_ShortUrl>>> Update(long id, [FromBody] Dto_UpdateShortUrl dto)
        {
            try
            {
                if (id != dto.ShortUrlId)
                {
                    return BadRequest(DataResult<Dto_ShortUrl>.FailResult("Id mismatched", StatusCodes.Status400BadRequest));
                }
                   

                if (!ModelState.IsValid)
                {
                    return BadRequest(DataResult<Dto_ShortUrl>.FailResult("Invalid input", StatusCodes.Status400BadRequest));
                }
                   

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<Dto_ShortUrl>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }
                    
                int userId = int.Parse(userIdClaim);

                var updated = await _shortUrlService.UpdateShortUrlAsync(dto, userId);
                if (updated == null)
                {
                    return NotFound(DataResult<Dto_ShortUrl>.FailResult("Not found", StatusCodes.Status404NotFound));
                }
                    

                return Ok(DataResult<Dto_ShortUrl>.SuccessResult(updated, "Updated", StatusCodes.Status200OK));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(DataResult<Dto_ShortUrl>.FailResult(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_ShortUrl>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
        [Authorize]
        [HttpPatch("{id:long}/activate")]
        public async Task<ActionResult> Activate(long id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }
                    

                int userId = int.Parse(userIdClaim);
                var success = await _shortUrlService.SetActiveStatusAsync(id, true, userId);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<object>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
        [Authorize]
        [HttpPatch("{id:long}/deactivate")]
        public async Task<ActionResult> Deactivate(long id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }
                  
                int userId = int.Parse(userIdClaim);
                var success = await _shortUrlService.SetActiveStatusAsync(id, false, userId);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<object>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
        [Authorize]
        [HttpDelete("delete/{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }
                   
                int userId = int.Parse(userIdClaim);
                var deleted = await _shortUrlService.DeleteShortUrlAsync(id, userId);
                if (!deleted) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<object>.FailResult(ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
    }
}
