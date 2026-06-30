using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class TourLogService : ITourLogService
{
    //ja vincent, hattest recht, wir brauchen eine Dictionary in Dictionary XD
    private static Dictionary<int, Dictionary<int, TourLogDTO>> _tourLogs = new Dictionary<int, Dictionary<int, TourLogDTO>>();
    private static int tourLogId = 0;
    public List<TourLogDTO> GetTourLogs(int tourId)
    {
        if (!_tourLogs.ContainsKey(tourId)||_tourLogs[tourId].Count == 0)
        {
            return new List<TourLogDTO>();
        }
        return _tourLogs[tourId].Values.ToList();
    }

    public bool AddTourLog(int tourId, TourLogDTO tourLog)
    {
        tourLog.ID = tourLogId++;
        if (!_tourLogs.ContainsKey(tourId))
        {
            _tourLogs.Add(tourId, new Dictionary<int, TourLogDTO>());
        }
        if(_tourLogs[tourId].ContainsKey(tourLog.ID))
        {
            return false;
        }
        
        _tourLogs[tourId].Add(tourLog.ID, tourLog);
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

    public bool UpdateTourLog(int tourId, int tourLogId, TourLogDTO tourLog)
    {
        if (!_tourLogs.ContainsKey(tourId) || !_tourLogs[tourId].ContainsKey(tourLog.ID))
        {
            return false;
        }
        
        tourLog.ID = tourLogId;
        
        _tourLogs[tourId][tourLogId] = tourLog;

        return true;
    }
}