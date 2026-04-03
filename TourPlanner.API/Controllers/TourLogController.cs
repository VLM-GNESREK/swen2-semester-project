using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;
[ApiController]
[Route("api/tours/{tourId:int}/logs")]
public class TourLogController : ControllerBase
{
    private readonly ITourLogService _tourLogService;

    public TourLogController(ITourLogService tourLogService)
    {
        _tourLogService = tourLogService;
    }


    [HttpGet]
    public IActionResult GetAll(int tourId)
    {
        //im using the tourId defined in the Route!
        var tours = _tourLogService.GetTourLogs(tourId:tourId);
        if (tours.Count==0)
        {
            return NoContent();
        }
        return Ok(tours);
    }

    [HttpPost]
    public IActionResult Create(int tourId, TourLogDto tour)
    {
        var addTour = _tourLogService.AddTourLog(tourId, tour);
        return Ok();
    }

    [HttpPut("{tourLogId}")]
    public IActionResult Update(int tourId,int tourLogId,TourLogDto tour)
    {
        if (_tourLogService.UpdateTourLog(tourId, tourLogId,tour))
        {
            return Ok();
        }
        return NotFound();
    }

    [HttpDelete("{tourLogId}")]
    public IActionResult Delete(int tourId,int tourLogId)
    {
        if (_tourLogService.DeleteTourLog(tourId:tourId,tourLogId))
        {
            return Ok();
        }
        return NotFound();
    }
}