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
        //[HttpGet("/r/{shortCode}")]
        //public async Task<ActionResult> RedirectToOriginal(string shortCode)
        //{
        //    var originalUrl = await _shortUrlService.GetByShortCodeAsync(shortCode);

        //    if (originalUrl == null)
        //    {
        //        return NotFound(DataResult<IEnumerable<Dto_ShortUrl>>.FailResult("URL Not Found", StatusCodes.Status404NotFound));
        //    }
                

        //    return Redirect(originalUrl.OriginalUrl);
        //}

    }
}
