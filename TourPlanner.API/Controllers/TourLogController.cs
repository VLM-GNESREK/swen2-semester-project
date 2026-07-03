using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers
{
    [ApiController]
    [Route("api/tours/{tourId:int}/logs")]
    //[Authorize]
    public class TourLogController : ControllerBase
    {
        private readonly ITourLogService _tourLogService;

        public TourLogController(ITourLogService tourLogService)
        {
            _tourLogService = tourLogService;
        }

        private int GetUserID()
        {
            var userIDClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userIDClaim == null)
            {
                throw new UnauthorizedAccessException("Unauthorised: User ID claim not found. (API1)");
            }
            return int.Parse(userIDClaim);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int tourID)
        {
            var tourLogs = await _tourLogService.GetTourLogsAsync(tourID, GetUserID());
            return Ok(tourLogs);
        }

        [HttpGet("{logID}")]
        public async Task<IActionResult> GetByID(int logID)
        {
            var tourLog = await _tourLogService.GetTourLogByTourLogIDAsync(logID, GetUserID());
            return Ok(tourLog);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tourID, [FromBody] TourLogDTO tourLog)
        {
            var createdLog = await _tourLogService.AddTourLogAsync(tourLog, tourID, GetUserID());
            return Ok(createdLog);
        }

        [HttpPut("{tourLogId}")]
        public async Task<IActionResult> Update(int tourID ,int tourLogID, [FromBody]TourLogDTO tourLog)
        {
            await _tourLogService.UpdateTourLogAsync(tourLog, tourID, tourLogID, GetUserID());
            return Ok();
        }

        [HttpDelete("{tourLogID}")]
        public async Task<IActionResult> Delete(int tourID,int tourLogID)
        {
            await _tourLogService.DeleteTourLogAsync(tourID, tourLogID, GetUserID());
            return NoContent();
        }
    }
}