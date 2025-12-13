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
        [HttpGet("getsummary")]
        public async Task<ActionResult<DataResult<Dto_ClickSummary>>> GetSummary()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<string>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }

                int userId = int.Parse(userIdClaim);
                var summary = await _analyticsService.GetSummaryAsync(userId);
                return Ok(DataResult<Dto_ClickSummary>.SuccessResult(summary, "Success", StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.InnerException?.Message ?? ex.Message, StatusCodes.Status500InternalServerError));

            }
        }

        [HttpGet("getlast7days")]
        [Authorize]
        public async Task<ActionResult<DataResult<List<Dto_DailyClickCount>>>> GetLast7Days()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<string>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }

                int userId = int.Parse(userIdClaim);

                var data = await _analyticsService.GetLast7DaysActivityAsync(userId);

                return Ok(DataResult<List<Dto_DailyClickCount>>.SuccessResult(data,"Success"));
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.InnerException?.Message ?? ex.Message, StatusCodes.Status500InternalServerError));
            }
            
        }

    }

}
