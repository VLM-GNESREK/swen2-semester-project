using Moq;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Entities;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.Tests.BL
{
    [TestFixture]
    public class TourServiceTests
    {
        private Mock<ITourRepository> _mockTourRepository;
        private TourService _tourService;

        [SetUp]
        public void Setup()
        {
            _mockTourRepository = new Mock<ITourRepository>();
            _tourService = new TourService(_mockTourRepository.Object);
        }

        private TourDTO CreateValidTourDto(decimal distance = 5000, int duration = 3600)
        {
            return new TourDTO
            {
                Name = "Updated Tour Name",
                Description = "Updated Description",
                From = "City A",
                To = "City B",
                OpenRoute = new OpenRoute
                {
                    TransportType = "bicycle",
                    Distance = distance,
                    Duration = duration,
                    ToFromCoords = new ToFromCoords
                    {
                        FromCoord = new Coords { Name = "City A", Lat = 48.2M, Lng = 16.3M },
                        ToCoord = new Coords { Name = "City B", Lat = 47.2M, Lng = 11.4M }
                    }
                },
                ImageRouteInformation = "/new/path.png",
                Popularity = 8,
                IsChildFriendly = false
            };
        }

        private Tour CreateValidTourEntity(int id, int userId)
        {
            return new Tour
            {
                tour_id = id,
                user_id = userId,
                tour_name = "Original Tour",
                description = "Original Description",
                startLocation = "A",
                targetLocation = "B",
                transportType = "car",
                distance = 5000,
                estimatedTime = 3600,
                FromLat = 1.0M,
                FromLng = 1.0M,
                ToLat = 2.0M,
                ToLng = 2.0M,
                routeImagePath = "/old/path.png",
                popularity = 5,
                childFriendly = 1
            };
        }

        [TestCase(false, (string?)null)]
        [TestCase(true, "Failed to retrieve tours. (BL1)")]
        public async Task GetToursAsync_Scenarios(bool throwRepoException, string? expectedExceptionMsg)
        {
            int userId = 1;
            if (throwRepoException)
            {
                _mockTourRepository.Setup(r => r.GetToursByUserIDAsync(userId))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                var tours = new List<Tour> { CreateValidTourEntity(1, userId) };
                _mockTourRepository.Setup(r => r.GetToursByUserIDAsync(userId))
                                   .ReturnsAsync(tours);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourService.GetToursAsync(userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _tourService.GetToursAsync(userId);
                var tourList = result.ToList();
                
                Assert.That(tourList.Count, Is.EqualTo(1));
                
                var dto = tourList.First();
                Assert.That(dto.ID, Is.EqualTo(1));
                Assert.That(dto.Name, Is.EqualTo("Original Tour"));
                Assert.That(dto.From, Is.EqualTo("A"));
                Assert.That(dto.OpenRoute.TransportType, Is.EqualTo("car"));
                Assert.That(dto.OpenRoute.Distance, Is.EqualTo(5000));
                Assert.That(dto.OpenRoute.ToFromCoords.FromCoord.Lat, Is.EqualTo(1.0M));
                Assert.That(dto.IsChildFriendly, Is.True);
            }
        }

        [TestCase(1, 1, true, true, false, (string?)null)] 
        [TestCase(1, 1, false, true, false, (string?)null)] 
        [TestCase(1, 2, true, false, false, "Forbidden: Tour unable to be retrieved due to insufficient permissions. (BL2)")] 
        [TestCase(1, 1, true, true, true, "Failed to retrieve tour. (BL2)")] 
        public async Task GetTourByIDAsync_Scenarios(int requestedTourId, int requestingUserId, bool tourExists, bool isOwner, bool throwRepoException, string? expectedExceptionMsg)
        {
            var tour = tourExists ? CreateValidTourEntity(requestedTourId, isOwner ? requestingUserId : 999) : null;
            
            if (throwRepoException)
            {
                _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(requestedTourId))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(requestedTourId))
                                   .ReturnsAsync(tour);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourService.GetTourByIDAsync(requestedTourId, requestingUserId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else if (!tourExists)
            {
                var result = await _tourService.GetTourByIDAsync(requestedTourId, requestingUserId);
                Assert.That(result, Is.Null);
            }
            else
            {
                var result = await _tourService.GetTourByIDAsync(requestedTourId, requestingUserId);
                
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.ID, Is.EqualTo(requestedTourId));
                Assert.That(result.Name, Is.EqualTo("Original Tour"));
                Assert.That(result.To, Is.EqualTo("B"));
                Assert.That(result.ImageRouteInformation, Is.EqualTo("/old/path.png"));
                Assert.That(result.OpenRoute.ToFromCoords.ToCoord.Lng, Is.EqualTo(2.0M));
            }
        }

        [TestCase(5000, 5000, true, false, (string?)null)] 
        [TestCase(15000, 5000, false, false, (string?)null)] 
        [TestCase(5000, 15000, false, false, (string?)null)] 
        [TestCase(5000, 5000, false, true, "Failed to add tour. (BL3)")] 
        public async Task AddTourAsync_Scenarios(decimal distance, int duration, bool expectedChildFriendly, bool throwRepoException, string? expectedExceptionMsg)
        {
            int userId = 1;
            var dto = CreateValidTourDto(distance, duration);

            if (throwRepoException)
            {
                _mockTourRepository.Setup(r => r.AddTourAsync(It.IsAny<Tour>()))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourRepository.Setup(r => r.AddTourAsync(It.IsAny<Tour>()))
                                   .Callback<Tour>(t => t.tour_id = 99) 
                                   .ReturnsAsync((Tour t) => t);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourService.AddTourAsync(dto, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _tourService.AddTourAsync(dto, userId);
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ID, Is.EqualTo(99));
                Assert.That(result.IsChildFriendly, Is.EqualTo(expectedChildFriendly));
                _mockTourRepository.Verify(r => r.AddTourAsync(It.IsAny<Tour>()), Times.Once);
            }
        }

        [TestCase(true, true, false, (string?)null)] 
        [TestCase(false, true, false, "Not Found: Tour not found. (BL4)")] 
        [TestCase(true, false, false, "Unauthorised: User does not have permission to update this tour. (BL5)")] 
        [TestCase(true, true, true, "Failed to update tour. (BL6)")] 
        public async Task UpdateTourAsync_Scenarios(bool tourExists, bool isOwner, bool throwRepoException, string? expectedExceptionMsg)
        {
            int tourId = 1;
            int userId = 1;
            var dto = CreateValidTourDto();
            var existingTour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;

            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(existingTour);

            if (throwRepoException)
            {
                _mockTourRepository.Setup(r => r.UpdateTourAsync(It.IsAny<Tour>()))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourRepository.Setup(r => r.UpdateTourAsync(It.IsAny<Tour>())).Returns(Task.CompletedTask);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourService.UpdateTourAsync(dto, tourId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                await _tourService.UpdateTourAsync(dto, tourId, userId);
                
                Assert.That(existingTour!.tour_name, Is.EqualTo(dto.Name));
                Assert.That(existingTour.startLocation, Is.EqualTo(dto.From));
                Assert.That(existingTour.targetLocation, Is.EqualTo(dto.To));
                Assert.That(existingTour.transportType, Is.EqualTo(dto.OpenRoute.TransportType));
                Assert.That(existingTour.FromLat, Is.EqualTo(dto.OpenRoute.ToFromCoords.FromCoord.Lat));
                Assert.That(existingTour.childFriendly, Is.EqualTo(0)); // Maps false to 0

                _mockTourRepository.Verify(r => r.UpdateTourAsync(existingTour), Times.Once);
            }
        }

        [TestCase(true, true, false, (string?)null)] 
        [TestCase(false, true, false, "Not Found: Tour not found. (BL7)")] 
        [TestCase(true, false, false, "Unauthorised: User does not have permission to delete this tour. (BL8)")] 
        [TestCase(true, true, true, "Failed to delete tour. (BL9)")] 
        public async Task DeleteTourAsync_Scenarios(bool tourExists, bool isOwner, bool throwRepoException, string? expectedExceptionMsg)
        {
            int tourId = 1;
            int userId = 1;
            var existingTour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;

            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(existingTour);

            if (throwRepoException)
            {
                _mockTourRepository.Setup(r => r.DeleteTourAsync(tourId))
                                   .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourRepository.Setup(r => r.DeleteTourAsync(tourId)).Returns(Task.CompletedTask);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourService.DeleteTourAsync(tourId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                await _tourService.DeleteTourAsync(tourId, userId);
                _mockTourRepository.Verify(r => r.DeleteTourAsync(tourId), Times.Once);
            }
        }
    }
}