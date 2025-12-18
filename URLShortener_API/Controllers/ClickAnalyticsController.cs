using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Application.Services;
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
        public async Task<ActionResult> GetSummary()
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
        public async Task<ActionResult> GetLast7Days()
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

        [HttpGet("topurls")]
        [Authorize]
        public async Task<ActionResult> GetTopUrls()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<string>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }

                int userId = int.Parse(userIdClaim);

                var data = await _analyticsService.GetTopPerformingUrlsAsync(userId, 3);

                return Ok(DataResult<List<Dto_TopPerformingUrl>>.SuccessResult(data,"Success"));
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.InnerException?.Message ?? ex.Message, StatusCodes.Status500InternalServerError));
            }
            
        }
        [HttpGet("devicetypes")]
        [Authorize]
        public async Task<ActionResult> GetDeviceTypes()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<string>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }

                int userId = int.Parse(userIdClaim);
                var data = await _analyticsService.GetDeviceTypeStatsAsync(userId);

                return Ok(DataResult<List<Dto_DeviceTypeStats>>.SuccessResult(data, "Success"));
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.InnerException?.Message ?? ex.Message, StatusCodes.Status500InternalServerError));
            }
            
        }
        [HttpGet("topcountries")]
        [Authorize]
        public async Task<ActionResult> GetTopCountries()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(DataResult<string>.FailResult("Unauthorized", StatusCodes.Status401Unauthorized));
                }

                int userId = int.Parse(userIdClaim);

                var result = await _analyticsService.GetTopCountriesAsync(userId, 5);

                return Ok(DataResult<List<Dto_CountryAnalytics>>.SuccessResult(result));
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, DataResult<string>.FailResult(ex.InnerException?.Message ?? ex.Message, StatusCodes.Status500InternalServerError));
            }
        }



    }

}
