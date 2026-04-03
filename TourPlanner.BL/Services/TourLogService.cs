using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class TourLogService : ITourLogService
{
    //ja vincent, hattest recht, wir brauchen eine Dictionary in Dictionary XD
    private static Dictionary<int, Dictionary<int, TourLogDto>> _tourLogs = new Dictionary<int, Dictionary<int, TourLogDto>>();
    private static int tourLogId = 0;
    public List<TourLogDto> GetTourLogs(int tourId)
    {
        if (!_tourLogs.ContainsKey(tourId)||_tourLogs[tourId].Count == 0)
        {
            return new List<TourLogDto>();
        }
        return _tourLogs[tourId].Values.ToList();
    }

    public bool AddTourLog(int tourId, TourLogDto tourLog)
    {
        tourLog.Id = tourLogId++;
        if (!_tourLogs.ContainsKey(tourId))
        {
            _tourLogs.Add(tourId, new Dictionary<int, TourLogDto>());
        }
        if(_tourLogs[tourId].ContainsKey(tourLog.Id))
        {
            return false;
        }
        
        _tourLogs[tourId].Add(tourLog.Id, tourLog);
        return true;
    }

    public bool DeleteTourLog(int tourId, int tourLogId)
    {
        if (!_tourLogs.ContainsKey(tourId))
        {
            return false;
        }
        return _tourLogs[tourId].Remove(tourLogId);
    }

    public bool UpdateTourLog(int tourId, int tourLogId, TourLogDto tourLog)
    {
        if (!_tourLogs.ContainsKey(tourId) || !_tourLogs[tourId].ContainsKey(tourLog.Id))
        {
            return false;
        }
        
        tourLog.Id = tourLogId;
        
        _tourLogs[tourId][tourLogId] = tourLog;

        return true;
    }
}