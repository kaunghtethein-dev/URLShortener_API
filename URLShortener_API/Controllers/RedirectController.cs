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
            try
            {
                var ip = GetClientIp();
                string referrer = Request.Headers["Referer"].ToString();
                string userAgent = Request.Headers["User-Agent"].ToString();


                var url = await _shortUrlService.HandleRedirectAsync(shortCode,ip, referrer, userAgent);

                if (url == null)
                {
                    return NotFound("Short link not found");
                }

                return Redirect(url);
            }
            catch (InvalidOperationException ex) when (ex.Message == "EXPIRED")
            {
                return StatusCode(StatusCodes.Status410Gone, "This short URL has expired");
            }
        }

        //private string? GetClientIp()
        //{
        //    if (Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
        //    {
        //        return cfIp.ToString();
        //    }
        //    return HttpContext.Connection.RemoteIpAddress?.ToString();
        //}
        private string? GetClientIp()
        {
            if (Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
            {
                return cfIp.FirstOrDefault();
            }

            if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var ipList = forwardedFor.FirstOrDefault()?.Split(',');
                return ipList?.FirstOrDefault()?.Trim();
            }
            if (Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            {
                return realIp.FirstOrDefault();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
