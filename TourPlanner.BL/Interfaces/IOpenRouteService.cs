using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface IOpenRouteService
{
    public Task<List<Coords>> GetStringToCoordinate(string searchString);
    public Task<OpenRoute> GetRoute(OpenRoute openRoute);
}