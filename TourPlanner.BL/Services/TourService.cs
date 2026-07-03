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

        public async Task<IEnumerable<TourDTO>> GetToursAsync(int userId)
        {
            try
            {
                var tours = await _tourRepository.GetToursByUserIDAsync(userId);

                return tours.Select(t => new TourDTO
                {
                    ID = t.tour_id,
                    Name = t.tour_name,
                    Description = t.description ?? string.Empty,
                    From = t.startLocation,
                    To = t.targetLocation,
                    OpenRoute = new OpenRoute
                    {
                        TransportType = t.transportType,
                        Distance = t.distance,
                        Duration = t.estimatedTime,
                        ToFromCoords = new ToFromCoords
                        {
                            FromCoord = new Coords
                            {
                                Name = t.startLocation,
                                Lat = t.FromLat,
                                Lng = t.FromLng
                            },
                            ToCoord = new Coords
                            {
                                Name = t.targetLocation,
                                Lat = t.ToLat,
                                Lng = t.ToLng
                            }
                        }
                    },

                    ImageRouteInformation = t.routeImagePath,
                    Popularity = t.popularity ?? 0,
                    IsChildFriendly = t.childFriendly == 1
                }).ToList();
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to retrieve tours. (BL1)", ex);
            }
        }

        public async Task<TourDTO?> GetTourByIDAsync(int tourId, int userId)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourId);

                if (tour == null)
                {
                    return null;
                }

                if (tour.user_id != userId)
                {
                    throw new BusinessException(
                        "Forbidden: Tour unable to be retrieved due to insufficient permissions. (BL2)");
                }

                return new TourDTO
                {
                    ID = tour.tour_id,
                    Name = tour.tour_name,
                    Description = tour.description ?? string.Empty,
                    From = tour.startLocation,
                    To = tour.targetLocation,
                    OpenRoute = new OpenRoute
                    {
                        TransportType = tour.transportType,
                        Distance = tour.distance,
                        Duration = tour.estimatedTime,
                        ToFromCoords = new ToFromCoords
                        {
                            FromCoord = new Coords
                            {
                                Name = tour.startLocation,
                                Lat = tour.FromLat,
                                Lng = tour.FromLng
                            },
                            ToCoord = new Coords
                            {
                                Name = tour.targetLocation,
                                Lat = tour.ToLat,
                                Lng = tour.ToLng
                            }
                        }
                    },
                    ImageRouteInformation = tour.routeImagePath,
                    Popularity = tour.popularity ?? 0,
                    IsChildFriendly = tour.childFriendly == 1
                };
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to retrieve tour. (BL2)", ex);
            }
        }

        public async Task<TourDTO> AddTourAsync(TourDTO tourDto, int userId)
        {
            const decimal maxChildFriendlyDistance = 10000;
            const int maxChildFriendlyTime = 10800;

            try
            {
                bool initialChildFriendly = tourDto.OpenRoute.Distance <= maxChildFriendlyDistance &&
                                            tourDto.OpenRoute.Duration <= maxChildFriendlyTime;

                var tour = new Tour
                {
                    tour_name = tourDto.Name,
                    description = tourDto.Description,
                    startLocation = tourDto.From,
                    targetLocation = tourDto.To,
                    transportType = tourDto.OpenRoute.TransportType,
                    distance = tourDto.OpenRoute.Distance,
                    estimatedTime = tourDto.OpenRoute.Duration,
                    FromLat = tourDto.OpenRoute.ToFromCoords.FromCoord.Lat,
                    FromLng = tourDto.OpenRoute.ToFromCoords.FromCoord.Lng,
                    ToLat = tourDto.OpenRoute.ToFromCoords.ToCoord.Lat,
                    ToLng = tourDto.OpenRoute.ToFromCoords.ToCoord.Lng,
                    routeImagePath = tourDto.ImageRouteInformation,
                    popularity = tourDto.Popularity,
                    childFriendly = initialChildFriendly ? 1 : 0,
                    user_id = userId
                };

                await _tourRepository.AddTourAsync(tour);

                return new TourDTO
                {
                    ID = tour.tour_id,
                    Name = tour.tour_name,
                    Description = tour.description ?? string.Empty,
                    From = tour.startLocation,
                    To = tour.targetLocation,
                    OpenRoute = new OpenRoute
                    {
                        TransportType = tour.transportType,
                        Distance = tour.distance,
                        Duration = tour.estimatedTime,
                        ToFromCoords = new ToFromCoords
                        {
                            FromCoord = new Coords
                            {
                                Name = tour.startLocation,
                                Lat = tour.FromLat,
                                Lng = tour.FromLng
                            },
                            ToCoord = new Coords
                            {
                                Name = tour.targetLocation,
                                Lat = tour.ToLat,
                                Lng = tour.ToLng
                            }
                        }
                    },
                    ImageRouteInformation = tour.routeImagePath,
                    Popularity = tour.popularity ?? 0,
                    IsChildFriendly = tour.childFriendly == 1
                };
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to add tour. (BL3)", ex);
            }
        }

        public async Task UpdateTourAsync(TourDTO tourDto, int tourId, int userId)
        {
            try
            {
                var existingTour = await _tourRepository.GetTourByTourIDAsync(tourId);

                if (existingTour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL4)");
                }

                if (existingTour.user_id != userId)
                {
                    throw new BusinessException(
                        "Unauthorised: User does not have permission to update this tour. (BL5)");
                }

                existingTour.tour_name = tourDto.Name;
                existingTour.description = tourDto.Description;
                existingTour.startLocation = tourDto.From;
                existingTour.targetLocation = tourDto.To;
                existingTour.transportType = tourDto.OpenRoute.TransportType;
                existingTour.distance = tourDto.OpenRoute.Distance;
                existingTour.estimatedTime = tourDto.OpenRoute.Duration;
                existingTour.FromLat = tourDto.OpenRoute.ToFromCoords.FromCoord.Lat;
                existingTour.FromLng = tourDto.OpenRoute.ToFromCoords.FromCoord.Lng;
                existingTour.ToLat = tourDto.OpenRoute.ToFromCoords.ToCoord.Lat;
                existingTour.ToLng = tourDto.OpenRoute.ToFromCoords.ToCoord.Lng;
                existingTour.routeImagePath = tourDto.ImageRouteInformation;
                existingTour.popularity = tourDto.Popularity;
                existingTour.childFriendly = tourDto.IsChildFriendly ? 1 : 0;

                await _tourRepository.UpdateTourAsync(existingTour);
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to update tour. (BL6)", ex);
            }
        }

        public async Task DeleteTourAsync(int tourId, int userId)
        {
            try
            {
                var existingTour = await _tourRepository.GetTourByTourIDAsync(tourId);


                if (existingTour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL7)");
                }

                if (existingTour.user_id != userId)
                {
                    throw new BusinessException(
                        "Unauthorised: User does not have permission to delete this tour. (BL8)");
                }

                await _tourRepository.DeleteTourAsync(tourId);
            }
            catch (DataAccessException ex)
            {
                throw new BusinessException("Failed to delete tour. (BL9)", ex);
            }
        }
    }
}