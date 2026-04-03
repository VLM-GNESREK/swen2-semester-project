using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface ITourService
{
   
    
    public List<TourDto> GetTours();
    public bool AddTour(TourDto tour);
    public bool DeleteToru(int id);
    public bool UpdateTour(int id, TourDto tour);
}