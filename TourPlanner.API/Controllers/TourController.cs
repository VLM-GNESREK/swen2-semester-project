using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/tours")]
[Authorize]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly IOpenRouteService _openRouteService;

    public TourController(ITourService tourService, IOpenRouteService openRouteService)
    {
        _tourService = tourService;
        _openRouteService = openRouteService;
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
    public async Task<IActionResult> GetAll()
    {
        var tours = await _tourService.GetToursAsync(GetUserID());
        return Ok(tours);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(int id)
    {
        var tour = await _tourService.GetTourByIDAsync(id, GetUserID());
        if(tour == null)
        {
            return NotFound( new { Error = "Tour not found." });
        }
        return Ok(tour);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TourDTO tour)
    {
        var updatedOpenRoute =  await _openRouteService.GetRoute(tour.OpenRoute);
        tour.OpenRoute = updatedOpenRoute;
        var createdTour = await _tourService.AddTourAsync(tour, GetUserID());
        return Ok(createdTour);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TourDTO tour)
    {
        await _tourService.UpdateTourAsync(tour, id, GetUserID());
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tourService.DeleteTourAsync(id, GetUserID());
        return NoContent();
    }
}