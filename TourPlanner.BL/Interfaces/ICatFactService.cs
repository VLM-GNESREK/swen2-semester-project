using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface ICatFactService
{
    Task<FactDto> CollectFactAsync(string username);
}