using TourPlanner.DAL.Entities;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllToursAsync();
        Task<Tour?> GetTourByTourIDAsync(int tourID);
        Task<IEnumerable<Tour>> GetToursByUserIDAsync(int userID);
        Task<Tour> AddTourAsync(Tour tour);
        Task UpdateTourAsync(Tour tour);
        Task DeleteTourAsync(int tourID);
    }
}