using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/tours")]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        var tours = _tourService.GetTours();
        if (tours.Count==0)
        {
            return NoContent();
        }
        return Ok(tours);
    }

    [HttpPost]
    public IActionResult Create(TourDto tour)
    {
        var addTour = _tourService.AddTour(tour);
        return Ok(addTour);
    }

    [HttpPut("{id}")]
    public IActionResult Update(TourDto tour)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}