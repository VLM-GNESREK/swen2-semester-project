using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;
[ApiController]
[Route("api/openroute")]
public class OpenRouteController : ControllerBase
{
    private readonly IOpenRouteService _openRouteService;

    public OpenRouteController(IOpenRouteService openRouteService)
    {
        _openRouteService = openRouteService;
    }
    [HttpGet("to/{toSearchString}")]
    public async Task<IActionResult> GetToSearchResult(string toSearchString)
    {
        var result = await _openRouteService.GetStringToCoordinate(toSearchString);
        if (result.Count == 0)
        {
            return NoContent();
        }
        return Ok(result);
    }    
    [HttpGet("from/{fromSearchString}")]
    public async Task<IActionResult> GetFromSearchResult(string fromSearchString)
    {
        var result = await _openRouteService.GetStringToCoordinate(fromSearchString);

        if (result.Count == 0)
            return NoContent();

        return Ok(result);
        
    }
    
    
}