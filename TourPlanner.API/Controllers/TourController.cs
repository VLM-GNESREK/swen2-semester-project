using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/tours")]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly IOpenRouteService _openRouteService;

    public TourController(ITourService tourService, IOpenRouteService openRouteService)
    {
        _tourService = tourService;
        _openRouteService = openRouteService;
    }


    [HttpGet("{id}")]
    public IActionResult GetbyId(int id)
    {
        var tours = _tourService.GetById(id);
        if (tours == null)
        {
            return NoContent();
        }
        return Ok(tours);
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
    public async Task<IActionResult> Create(TourDto tour)
    {
        
        var updatedOpenRoute =  await _openRouteService.GetRoute(tour.OpenRoute);
        tour.OpenRoute = updatedOpenRoute;
        var addTour = _tourService.AddTour(tour);
        return Ok(addTour);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id,TourDto tour)
    {
        if (_tourService.UpdateTour(id, tour))
        {
            return Ok();
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_tourService.DeleteToru(id))
        {
            return Ok();
        }
        return NotFound();
    }
}