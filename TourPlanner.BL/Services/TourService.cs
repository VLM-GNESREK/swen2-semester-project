using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Interfaces;
using TourPlanner.DAL.Exceptions;
using TourPlanner.DAL.Repositories;

namespace TourPlanner.BL.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        public TourService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public async Task<IEnumerable<TourDTO>> GetToursAsync(int userID)
        {
            try
            {
                var tours = await _tourRepository.GetAllToursAsync();

                return tours.Where(t => t.user_id == userID).Select(t => new TourDTO
                {
                    ID = t.tour_id,
                    Name = t.tour_name,
                    Description = t.description ?? string.Empty,
                    From = t.startLocation,
                    To = t.targetLocation,
                    TransportType = t.transportType,
                    Distance = t.distance,
                    EstimatedTime = t.estimatedTime,
                    ImageRouteInformation = t.routeImagePath,
                    Popularity = t.popularity ?? 0,
                    IsChildFriendly = t.childFriendly == 1
                }).ToList();
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to retrieve tours. (BL1)", ex);
            }
        }

        public async Task<TourDTO?> GetTourByIDAsync(int tourID, int userID)
        {
            try
            {
                var tour = await _tourRepository.GetTourByIDAsync(tourID);

                if(tour == null || tour.user_id != userID)
                {
                    return null;
                }

                return new TourDTO
                {
                    ID = tour.tour_id,
                    Name = tour.tour_name,
                    Description = tour.description ?? string.Empty,
                    From = tour.startLocation,
                    To = tour.targetLocation,
                    TransportType = tour.transportType,
                    Distance = tour.distance,
                    EstimatedTime = tour.estimatedTime,
                    ImageRouteInformation = tour.routeImagePath,
                    Popularity = tour.popularity ?? 0,
                    IsChildFriendly = tour.childFriendly == 1
                };
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to retrieve tour. (BL2)", ex);
            }
        }


    }
}