using TourPlanner.DAL.Entities;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllToursAsync();
        Task<Tour?> GetTourByIDAsync(int tourID);
        Task<Tour> AddTourAsync(Tour tour);
        Task UpdateTourAsync(Tour tour);
        Task DeleteTourAsync(int tourID);
    }
}