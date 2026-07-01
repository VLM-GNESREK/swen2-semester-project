using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Interfaces;
using TourPlanner.DAL.Entities;
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
                var tours = await _tourRepository.GetToursByUserIDAsync(userID);

                return tours.Select(t => new TourDTO
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
                var tour = await _tourRepository.GetTourByTourIDAsync(tourID);

                if(tour == null)
                {
                    return null;
                }
                if(tour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: Tour unable to be retrieved due to insufficient permissions. (BL2)");
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

        public async Task<TourDTO> AddTourAsync(TourDTO tourDTO, int userID)
        {
            try
            {
                var tour = new Tour
                {
                    tour_name = tourDTO.Name,
                    description = tourDTO.Description,
                    startLocation = tourDTO.From,
                    targetLocation = tourDTO.To,
                    transportType = tourDTO.TransportType,
                    distance = tourDTO.Distance,
                    estimatedTime = tourDTO.EstimatedTime,
                    routeImagePath = tourDTO.ImageRouteInformation,
                    popularity = tourDTO.Popularity,
                    childFriendly = tourDTO.IsChildFriendly ? 1 : 0,
                    user_id = userID
                };

                await _tourRepository.AddTourAsync(tour);

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
                throw new BusinessException("Failed to add tour. (BL3)", ex);
            }
        }

        public async Task UpdateTourAsync(TourDTO tourDTO, int tourID, int userID)
        {
            try
            {
                var existingTour = await _tourRepository.GetTourByTourIDAsync(tourID);

                if(existingTour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL4)");
                }
                if(existingTour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User does not have permission to update this tour. (BL5)");
                }

                existingTour.tour_name = tourDTO.Name;
                existingTour.description = tourDTO.Description;
                existingTour.startLocation = tourDTO.From;
                existingTour.targetLocation = tourDTO.To;
                existingTour.transportType = tourDTO.TransportType;
                existingTour.distance = tourDTO.Distance;
                existingTour.estimatedTime = tourDTO.EstimatedTime;
                existingTour.routeImagePath = tourDTO.ImageRouteInformation;
                existingTour.popularity = tourDTO.Popularity;
                existingTour.childFriendly = tourDTO.IsChildFriendly ? 1 : 0;

                await _tourRepository.UpdateTourAsync(existingTour);
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to update tour. (BL6)", ex);
            }
        }

        public async Task DeleteTourAsync(int tourID, int userID)
        {
            try
            {
                var existingTour = await _tourRepository.GetTourByTourIDAsync(tourID);


                if(existingTour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL7)");
                }
                if(existingTour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User does not have permission to delete this tour. (BL8)");
                }

                await _tourRepository.DeleteTourAsync(tourID);
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to delete tour. (BL9)", ex);
            }
        }
    }
}