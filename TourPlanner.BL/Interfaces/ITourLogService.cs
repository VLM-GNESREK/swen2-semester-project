using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces
{
    public interface ITourLogService
    {
        Task<IEnumerable<TourLogDTO>> GetTourLogsAsync(int tourID, int userID);
        Task<TourLogDTO?> GetTourLogByTourLogIDAsync(int logID, int userID);
        Task<TourLogDTO> AddTourLogAsync(TourLogDTO tourLog, int tourID, int userID);
        Task UpdateTourLogAsync(TourLogDTO tourLog, int tourID, int tourLogID, int userID);
        Task DeleteTourLogAsync(int tourID, int tourLogID, int userID);
    }
}