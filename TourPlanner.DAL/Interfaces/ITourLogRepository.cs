using TourPlanner.DAL.Entities;

namespace TourPlanner.DAL.Repositories
{
    public interface ITourLogRepository
    {
        Task<TourLog?> GetTourLogByIDAsync(int logID);
        Task<IEnumerable<TourLog>> GetTourLogsByTourIDAsync(int tourID);
        Task<TourLog> AddTourLogAsync(TourLog tourLog);
        Task UpdateTourLogAsync(TourLog tourLog);
        Task DeleteTourLogAsync(int logID);
    }
}