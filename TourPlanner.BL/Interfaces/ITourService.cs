using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces
{
    public interface ITourService
    {   
        Task<IEnumerable<TourDTO>> GetToursAsync(int userID);
        Task<TourDTO?> GetTourByIDAsync(int tourID, int userID);
        Task<TourDTO> AddTourAsync(TourDTO tourDTO, int userID);
        Task UpdateTourAsync(TourDTO tourDTO, int tourID, int userID);
        Task DeleteTourAsync(int tourID, int userID);
    }
}