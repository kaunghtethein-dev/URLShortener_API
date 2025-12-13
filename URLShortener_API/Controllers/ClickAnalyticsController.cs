using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Shared.DTOs;
using URLShortener_Shared.Wrappers;

namespace URLShortener_API.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class ClickAnalyticsController : ControllerBase
    {
        private readonly IClickAnalyticsService _analyticsService;
        public ClickAnalyticsController(IClickAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Authorize]
        [HttpGet("getsummary/{userId}")]
        public async Task<ActionResult<DataResult<Dto_ClickSummary>>> GetSummary(int userId)
        {
            try
            {
                var summary = await _analyticsService.GetSummaryAsync(userId);
                return Ok(DataResult<Dto_ClickSummary>.SuccessResult(summary, "Success", StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, DataResult<Dto_ShortUrl>.FailResult(ex.InnerException?.Message ?? ex.Message, StatusCodes.Status500InternalServerError));
                }

            }
        }
    }
}
