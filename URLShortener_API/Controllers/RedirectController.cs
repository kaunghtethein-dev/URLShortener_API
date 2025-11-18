using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Shared.Wrappers;

namespace URLShortener_API.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public RedirectController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        [HttpGet("{shortCode}")]
        public async Task<ActionResult> RedirectToOriginal(string shortCode)
        {
            var originalUrl = await _shortUrlService.GetByShortCodeAsync(shortCode);

            if (originalUrl == null)
            {
                return NotFound(DataResult<IEnumerable<string>>.FailResult("URL Not Found", StatusCodes.Status404NotFound));
            }
            if (!originalUrl.IsActive)
            {
                return StatusCode(StatusCodes.Status410Gone,
                    DataResult<string>.FailResult("This URL is inactive", StatusCodes.Status410Gone));
            }

            if (originalUrl.ExpiresAt != null && originalUrl.ExpiresAt <= DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status410Gone,
                    DataResult<string>.FailResult($"This URL has expired.", StatusCodes.Status410Gone));
            }
            return Redirect(originalUrl.OriginalUrl);
        }
    }
}
