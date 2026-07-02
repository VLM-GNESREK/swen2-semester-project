using TourPlanner.BL.Interfaces;
using TourPlanner.BL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace TourPlanner.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userRegistrationDTO)
        {
            await _userService.RegisterUserAsync(userRegistrationDTO);
            return Ok(new { Message = "Registration successful." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            var response = await _userService.LoginUserAsync(userLoginDTO);
            return Ok(response);
        }
    }
}