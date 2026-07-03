using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Entities;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.Tests.BL
{
    [TestFixture]
    public class TourLogServiceTests
    {
        private Mock<ITourLogRepository> _mockTourLogRepository;
        private Mock<ITourRepository> _mockTourRepository;
        private TourLogService _tourLogService;

        [SetUp]
        public void Setup()
        {
            _mockTourLogRepository = new Mock<ITourLogRepository>();
            _mockTourRepository = new Mock<ITourRepository>();
            _tourLogService = new TourLogService(_mockTourLogRepository.Object, _mockTourRepository.Object);
        }

        private Tour CreateValidTourEntity(int id, int userId, decimal distance = 5000, int estimatedTime = 3600)
        {
            return new Tour
            {
                tour_id = id,
                user_id = userId,
                distance = distance,
                estimatedTime = estimatedTime,
                popularity = 0,
                childFriendly = 1
            };
        }

        private TourLog CreateValidTourLogEntity(int logId, int tourId, int difficulty = 1)
        {
            return new TourLog
            {
                log_id = logId,
                tour_id = tourId,
                difficulty = difficulty,
                logDateTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Local),
                comment = "Great log",
                totalDistance = 10.5,
                totalTime = 120,
                rating = 5
            };
        }

        private TourLogDTO CreateValidTourLogDto()
        {
            return new TourLogDTO
            {
                Date = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Local),
                Comment = "Updated log",
                Difficulty = 3,
                TotalDistance = 15.0,
                TotalTime = 150,
                Rating = 4
            };
        }

        [TestCase(true, true, false, (string?)null)]
        [TestCase(false, true, false, "Not Found: Tour not found. (BL10)")]
        [TestCase(true, false, false, "Unauthorised: User has insufficient permissions to access this tour's logs. (BL11)")]
        [TestCase(true, true, true, "Failed to retrieve tour logs. (BL12)")]
        public async Task GetTourLogsAsync_Scenarios(bool tourExists, bool isOwner, bool throwRepoException, string? expectedExceptionMsg)
        {
            int tourId = 1, userId = 1;
            var tour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;
            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(tour);

            if (throwRepoException)
            {
                _mockTourLogRepository.Setup(r => r.GetTourLogsByTourIDAsync(tourId))
                                      .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                var logs = new List<TourLog> { CreateValidTourLogEntity(1, tourId) };
                _mockTourLogRepository.Setup(r => r.GetTourLogsByTourIDAsync(tourId)).ReturnsAsync(logs);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourLogService.GetTourLogsAsync(tourId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _tourLogService.GetTourLogsAsync(tourId, userId);
                var resultList = result.ToList();
                Assert.That(resultList.Count, Is.EqualTo(1));
                Assert.That(resultList.First().Comment, Is.EqualTo("Great log"));
            }
        }

        [TestCase(true, true, true, false, (string?)null)]
        [TestCase(false, true, true, false, (string?)null)]
        [TestCase(true, false, true, false, "Not Found: Tour not found. (BL13)")]
        [TestCase(true, true, false, false, "Unauthorised: User has insufficient permissions to access this tour log. (BL14)")]
        [TestCase(true, true, true, true, "Failed to retrieve tour log. (BL15)")]
        public async Task GetTourLogByTourLogIDAsync_Scenarios(bool logExists, bool tourExists, bool isOwner, bool throwRepoException, string? expectedExceptionMsg)
        {
            int logId = 1, tourId = 1, userId = 1;
            var log = logExists ? CreateValidTourLogEntity(logId, tourId) : null;
            var tour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;

            if (throwRepoException)
            {
                _mockTourLogRepository.Setup(r => r.GetTourLogByIDAsync(logId))
                                      .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourLogRepository.Setup(r => r.GetTourLogByIDAsync(logId)).ReturnsAsync(log);
            }

            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(tour);

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourLogService.GetTourLogByTourLogIDAsync(logId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else if (!logExists)
            {
                var result = await _tourLogService.GetTourLogByTourLogIDAsync(logId, userId);
                Assert.That(result, Is.Null);
            }
            else
            {
                var result = await _tourLogService.GetTourLogByTourLogIDAsync(logId, userId);
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.ID, Is.EqualTo(logId));
            }
        }

        [TestCase(true, true, false, 0, (string?)null)]
        [TestCase(true, true, false, 3, (string?)null)]
        [TestCase(false, true, false, 0, "Not Found: Tour not found. (BL16)")]
        [TestCase(true, false, false, 0, "Unauthorised: User has insufficient permissions to add a log to this tour. (BL17)")]
        [TestCase(true, true, true, 0, "Failed to add tour log. (BL18)")]
        public async Task AddTourLogAsync_Scenarios(bool tourExists, bool isOwner, bool throwRepoException, int existingLogDifficulty, string? expectedExceptionMsg)
        {
            int tourId = 1, userId = 1;
            var dto = CreateValidTourLogDto();
            var tour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;
            
            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(tour);

            var existingLogs = existingLogDifficulty > 0 
                ? new List<TourLog> { CreateValidTourLogEntity(2, tourId, existingLogDifficulty) } 
                : new List<TourLog>();
            _mockTourLogRepository.Setup(r => r.GetTourLogsByTourIDAsync(tourId)).ReturnsAsync(existingLogs);

            if (throwRepoException)
            {
                _mockTourLogRepository.Setup(r => r.AddTourLogAsync(It.IsAny<TourLog>()))
                                      .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourLogRepository.Setup(r => r.AddTourLogAsync(It.IsAny<TourLog>()))
                                      .Callback<TourLog>(l => l.log_id = 99)
                                      .ReturnsAsync((TourLog l) => l);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourLogService.AddTourLogAsync(dto, tourId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                if (!throwRepoException)
                {
                    _mockTourLogRepository.Verify(r => r.AddTourLogAsync(It.IsAny<TourLog>()), Times.Never);
                }
            }
            else
            {
                var result = await _tourLogService.AddTourLogAsync(dto, tourId, userId);
                
                Assert.That(result.ID, Is.EqualTo(99));
                
                _mockTourLogRepository.Verify(r => r.AddTourLogAsync(It.Is<TourLog>(l => l.logDateTime.Kind == DateTimeKind.Utc)), Times.Once);

                int expectedChildFriendly = existingLogDifficulty > 2 ? 0 : 1;
                _mockTourRepository.Verify(r => r.UpdateTourAsync(It.Is<Tour>(t => 
                    t.popularity == existingLogs.Count &&
                    t.childFriendly == expectedChildFriendly
                )), Times.Once);
            }
        }

        [TestCase(true, true, true, true, false, (string?)null)]
        [TestCase(false, true, true, true, false, "Not Found: Tour not found. (BL19)")]
        [TestCase(true, false, true, true, false, "Forbidden: User has insufficient permissions to update a log for this tour. (BL20)")]
        [TestCase(true, true, false, true, false, "Not Found: Tour log not found. (BL21)")]
        [TestCase(true, true, true, false, false, "Bad Request: Tour log does not belong to the specified tour. (BL22)")]
        [TestCase(true, true, true, true, true, "Failed to update tour log. (BL23)")]
        public async Task UpdateTourLogAsync_Scenarios(bool tourExists, bool isOwner, bool logExists, bool correctTourId, bool throwRepoException, string? expectedExceptionMsg)
        {
            int tourId = 1, logId = 1, userId = 1;
            var dto = CreateValidTourLogDto();
            var tour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;
            var existingLog = logExists ? CreateValidTourLogEntity(logId, correctTourId ? tourId : 999) : null;

            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(tour);
            _mockTourLogRepository.Setup(r => r.GetTourLogByIDAsync(logId)).ReturnsAsync(existingLog);
            _mockTourLogRepository.Setup(r => r.GetTourLogsByTourIDAsync(tourId)).ReturnsAsync(new List<TourLog>());

            if (throwRepoException)
            {
                _mockTourLogRepository.Setup(r => r.UpdateTourLogAsync(It.IsAny<TourLog>()))
                                      .ThrowsAsync(new DataAccessException("DB Error"));
            }
            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourLogService.UpdateTourLogAsync(dto, tourId, logId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                if (!throwRepoException)
                {
                    _mockTourLogRepository.Verify(r => r.UpdateTourLogAsync(It.IsAny<TourLog>()), Times.Never);
                }
            }
            else
            {
                await _tourLogService.UpdateTourLogAsync(dto, tourId, logId, userId);

                Assert.That(existingLog!.comment, Is.EqualTo(dto.Comment));
                Assert.That(existingLog.logDateTime.Kind, Is.EqualTo(DateTimeKind.Utc));

                _mockTourLogRepository.Verify(r => r.UpdateTourLogAsync(existingLog), Times.Once);
                _mockTourRepository.Verify(r => r.UpdateTourAsync(tour!), Times.Once);
            }
        }

        [TestCase(true, true, true, true, false, (string?)null)]
        [TestCase(false, true, true, true, false, "Not Found: Tour not found. (BL24)")]
        [TestCase(true, false, true, true, false, "Forbidden: User has insufficient permissions to delete a log for this tour. (BL25)")]
        [TestCase(true, true, false, true, false, "Not Found: Tour log not found. (BL26)")]
        [TestCase(true, true, true, false, false, "Bad Request: Tour log does not belong to the specified tour. (BL27)")]
        [TestCase(true, true, true, true, true, "Failed to delete tour log. (BL28)")]
        public async Task DeleteTourLogAsync_Scenarios(bool tourExists, bool isOwner, bool logExists, bool correctTourId, bool throwRepoException, string? expectedExceptionMsg)
        {
            int tourId = 1, logId = 1, userId = 1;
            var tour = tourExists ? CreateValidTourEntity(tourId, isOwner ? userId : 999) : null;
            var existingLog = logExists ? CreateValidTourLogEntity(logId, correctTourId ? tourId : 999) : null;

            _mockTourRepository.Setup(r => r.GetTourByTourIDAsync(tourId)).ReturnsAsync(tour);
            _mockTourLogRepository.Setup(r => r.GetTourLogByIDAsync(logId)).ReturnsAsync(existingLog);
            _mockTourLogRepository.Setup(r => r.GetTourLogsByTourIDAsync(tourId)).ReturnsAsync(new List<TourLog>());

            if (throwRepoException)
            {
                _mockTourLogRepository.Setup(r => r.DeleteTourLogAsync(logId))
                                      .ThrowsAsync(new DataAccessException("DB Error"));
            }
            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() => _tourLogService.DeleteTourLogAsync(tourId, logId, userId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
                if (!throwRepoException)
                {
                    _mockTourLogRepository.Verify(r => r.DeleteTourLogAsync(It.IsAny<int>()), Times.Never);
                }
            }
            else
            {
                await _tourLogService.DeleteTourLogAsync(tourId, logId, userId);

                _mockTourLogRepository.Verify(r => r.DeleteTourLogAsync(logId), Times.Once);
                _mockTourRepository.Verify(r => r.UpdateTourAsync(tour!), Times.Once); // Ensures recalculation happens
            }
        }
    }
}