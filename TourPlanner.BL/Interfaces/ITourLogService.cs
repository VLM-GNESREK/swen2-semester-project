using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface ITourLogService
{
    public List<TourLogDto> GetTourLogs(int tourId);
    public bool AddTourLog(int tourId, TourLogDto tourLog);
    public bool DeleteTourLog(int tourId, int tourLogId);
    public bool UpdateTourLog(int tourId, int tourLogId, TourLogDto tourLog);
}