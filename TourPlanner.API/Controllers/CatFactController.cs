using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;
[ApiController]
[Route("api/cat")]
public class CatFactController : ControllerBase
{

    private readonly ICatFactService _catFactService;


    public CatFactController(ICatFactService catFactService)
    {
        _catFactService = catFactService;
    }

    [HttpGet]
    public IActionResult GetCatFact()
    {
        var test = _catFactService.CollectFactAsync("test");
        
        return Ok(test.Result);
    }
}