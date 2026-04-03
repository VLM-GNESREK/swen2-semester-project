using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class TourService : ITourService
{
    //TODO implement DAL layer later on
    private static Dictionary<int, TourDto> _tours = new Dictionary<int, TourDto>();
    private static int _tourId = 0;
    
    public List<TourDto> GetTours()
    {
        return _tours.Values.ToList();
    }

    public bool AddTour(TourDto tour)
    {
        tour.Id = _tourId++;
        _tours.Add(tour.Id, tour);
        return true;
    }

    public bool DeleteToru(int id)
    {
        return _tours.Remove(id);
        
    }

    public bool UpdateTour(int id, TourDto tour)
    {
        if (_tours.ContainsKey(id))
        {
            tour.Id = id;
            _tours[id] = tour;
            return true;
        }
        return false;
    }
}