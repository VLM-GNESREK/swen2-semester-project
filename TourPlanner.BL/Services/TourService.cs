using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class TourService : ITourService
{
    //TODO implement DAL layer later on
    private static Dictionary<int, TourDTO> _tours = new Dictionary<int, TourDTO>();
    private static int _tourId = 0;
    
    public List<TourDTO> GetTours()
    {
        return _tours.Values.ToList();
    }

    public bool AddTour(TourDTO tour)
    {
        tour.ID = _tourId++;
        _tours.Add(tour.ID, tour);
        return true;
    }

    public bool DeleteToru(int id)
    {
        return _tours.Remove(id);
        
    }

    public bool UpdateTour(int id, TourDTO tour)
    {
        if (_tours.ContainsKey(id))
        {
            tour.ID = id;
            _tours[id] = tour;
            return true;
        }
        return false;
    }

    public TourDTO? GetById(int id)
    {
        if (_tours.ContainsKey(id))
        {
            return _tours[id];
        }

        return null;
    }
}