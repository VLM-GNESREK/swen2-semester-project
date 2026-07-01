using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserRegistrationDTO registrationDTO);
        Task<AuthResponseDTO> LoginUserAsync(string username, string password);
    }
}