using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface ITourService
{
   
    
    public List<TourDTO> GetTours();
    public bool AddTour(TourDTO tour);
    public bool DeleteToru(int id);
    public bool UpdateTour(int id, TourDTO tour);
    public TourDTO? GetById(int id);
}