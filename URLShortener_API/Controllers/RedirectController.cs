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
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var originalUrl = await _shortUrlService.GetByShortCodeAsync(shortCode);

            if (originalUrl == null)
                return NotFound(DataResult<IEnumerable<string>>.FailResult("URL Not Found", StatusCodes.Status404NotFound));

            return Redirect(originalUrl.OriginalUrl);
        }
    }
}
