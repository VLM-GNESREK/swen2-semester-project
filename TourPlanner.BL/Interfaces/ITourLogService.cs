using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface ITourLogService
{
    public List<TourLogDTO> GetTourLogs(int tourId);
    public bool AddTourLog(int tourId, TourLogDTO tourLog);
    public bool DeleteTourLog(int tourId, int tourLogId);
    public bool UpdateTourLog(int tourId, int tourLogId, TourLogDTO tourLog);
}