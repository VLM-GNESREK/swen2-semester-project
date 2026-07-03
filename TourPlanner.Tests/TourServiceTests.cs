using Moq;
using NUnit.Framework;
using TourPlanner.BL.Services;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Entities;
using TourPlanner.DAL.Exceptions;
using TourPlanner.DAL.Repositories;

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

        private Tour CreateTour(int id = 1, int userId = 1)
        {
            return new Tour
            {
                tour_id = id,
                user_id = userId,
                tour_name = "Test Tour",
                description = "Desc",
                startLocation = "A",
                targetLocation = "B",
                transportType = "car",
                distance = 5000,
                estimatedTime = 3600,
                FromLat = 1,
                FromLng = 1,
                ToLat = 2,
                ToLng = 2,
                popularity = 2,
                childFriendly = 1
            };
        }

        private TourDTO CreateDto()
        {
            return new TourDTO
            {
                Name = "New Tour",
                Description = "Desc",
                From = "A",
                To = "B",
                OpenRoute = new OpenRoute
                {
                    TransportType = "car",
                    Distance = 5000,
                    Duration = 3600,
                    ToFromCoords = new ToFromCoords
                    {
                        FromCoord = new Coords { Name = "A", Lat = 1, Lng = 1 },
                        ToCoord = new Coords { Name = "B", Lat = 2, Lng = 2 }
                    }
                },
                ImageRouteInformation = "/img.png",
                Popularity = 0,
                IsChildFriendly = true
            };
        }

        // GET ALL TOURS

        [Test]
        public async Task GetTours_ReturnsMappedTours()
        {
            _mockTourRepository
                .Setup(r => r.GetAllToursAsync())
                .ReturnsAsync(new List<Tour> { CreateTour() });

            var result = (await _tourService.GetToursAsync(1)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Test Tour"));
            Assert.That(result[0].From, Is.EqualTo("A"));
        }
        // GET TOURS WHEN REPO FAILS
        [Test]
        public void GetTours_WhenRepoFails_ThrowsBusinessException()
        {
            _mockTourRepository
                .Setup(r => r.GetAllToursAsync())
                .ThrowsAsync(new DataAccessException());

            Assert.ThrowsAsync<BusinessException>(() =>
                _tourService.GetToursAsync(1));
        }

        // GET TOUR BY SPECIFIC ID

        [Test]
        public async Task GetTourById_ReturnsTour()
        {
            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(1))
                .ReturnsAsync(CreateTour());

            var result = await _tourService.GetTourByIDAsync(1, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ID, Is.EqualTo(1));
        }
        // GETT TOUR BY ID BUT TOUR DOES NOT EXIST
        [Test]
        public async Task GetTourById_WhenNotFound_ReturnsNull()
        {
            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(1))
                .ReturnsAsync((Tour?)null);

            var result = await _tourService.GetTourByIDAsync(1, 1);

            Assert.That(result, Is.Null);
        }
        
        // ADD TOUR SUCCESFULLYY

        [Test]
        public async Task AddTour_CreatesTour()
        {
            _mockTourRepository
                .Setup(r => r.AddTourAsync(It.IsAny<Tour>()))
                .Callback<Tour>(t => t.tour_id = 99)
                .ReturnsAsync((Tour t) => t);

            var result = await _tourService.AddTourAsync(CreateDto(), 1);

            Assert.That(result.ID, Is.EqualTo(99));
            Assert.That(result.Name, Is.EqualTo("New Tour"));

            _mockTourRepository.Verify(r => r.AddTourAsync(It.IsAny<Tour>()), Times.Once);
        }
        // ADD TOUR BUT REPO FAILS
        [Test]
        public void AddTour_WhenRepoFails_Throws()
        {
            _mockTourRepository
                .Setup(r => r.AddTourAsync(It.IsAny<Tour>()))
                .ThrowsAsync(new DataAccessException());

            Assert.ThrowsAsync<BusinessException>(() =>
                _tourService.AddTourAsync(CreateDto(), 1));
        }

        // WE UPDATE TOUR AND IT WORKS

        [Test]
        public async Task UpdateTour_UpdatesTour()
        {
            var tour = CreateTour();

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(1))
                .ReturnsAsync(tour);

            _mockTourRepository
                .Setup(r => r.UpdateTourAsync(It.IsAny<Tour>()))
                .Returns(Task.CompletedTask);

            await _tourService.UpdateTourAsync(CreateDto(), 1, 1);

            Assert.That(tour.tour_name, Is.EqualTo("New Tour"));

            _mockTourRepository.Verify(r =>
                r.UpdateTourAsync(tour),
                Times.Once);
        }
        // WE UPDATE TOUR BUT ITS NOT THE OWNER OF TOUR
        [Test]
        public void UpdateTour_WhenNotOwner_Throws()
        {
            var tour = CreateTour(userId: 2);

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(1))
                .ReturnsAsync(tour);

            Assert.ThrowsAsync<BusinessException>(() =>
                _tourService.UpdateTourAsync(CreateDto(), 1, 1));
        }

        // WE DELETE TOUR

        [Test]
        public async Task DeleteTour_DeletesTour()
        {
            var tour = CreateTour();

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(1))
                .ReturnsAsync(tour);

            _mockTourRepository
                .Setup(r => r.DeleteTourAsync(1))
                .Returns(Task.CompletedTask);

            await _tourService.DeleteTourAsync(1, 1);

            _mockTourRepository.Verify(r =>
                r.DeleteTourAsync(1),
                Times.Once);
        }
        // DELETE TOUR BUT ITS ALREADY GONE
        [Test]
        public void DeleteTour_WhenNotFound_Throws()
        {
            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(1))
                .ReturnsAsync((Tour?)null);

            Assert.ThrowsAsync<BusinessException>(() =>
                _tourService.DeleteTourAsync(1, 1));
        }
    }
}