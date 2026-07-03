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
    public class TourLogServiceTests
    {
        private Mock<ITourLogRepository> _mockTourLogRepository;
        private Mock<ITourRepository> _mockTourRepository;
        private Mock<IUserRepository> _mockUserRepository;


        private TourLogService _tourLogService;

        [SetUp]
        public void Setup()
        {
            _mockTourLogRepository = new Mock<ITourLogRepository>();
            _mockTourRepository = new Mock<ITourRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _tourLogService = new TourLogService(_mockTourLogRepository.Object, _mockTourRepository.Object,
                _mockUserRepository.Object);
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
                comment = "Great log by Elmin",
                totalDistance = 1054,
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
                TotalDistance = 150,
                TotalTime = 150,
                Rating = 4
            };
        }

        private User CreateValidUser(int id, string username = "MegaElmin")
        {
            return new User
            {
                user_id = id,
                username = username
            };
        }
        // ------------------------------------------------------------
        // TESTING IF I CAN GET ALL TOURLOGS in BL
        // ------------------------------------------------------------

        //TOUR EXISTS == NORMAL OPERATION
        [TestCase(true, false, null)]
        //TOUR NOT EXISTS = NORMAL OPERATION
        [TestCase(false, false, "Not Found: Tour not found. (BL10)")]
        //TOUR LOGS WERE NOT FETCHABLE == PROBLEMO
        [TestCase(true, true, "Failed to retrieve tour logs. (BL12)")]
        public async Task GetTourLogsAsync_Scenarios(
            bool tourExists,
            bool throwRepoException,
            string? expectedExceptionMsg)
        {
            int tourId = 1, userId = 1;

            var tour = tourExists ? CreateValidTourEntity(tourId, 1) : null;
            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(tourId))
                .ReturnsAsync(tour);

            if (throwRepoException)
            {
                _mockTourLogRepository
                    .Setup(r => r.GetTourLogsByTourIDAsync(tourId))
                    .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                var logs = new List<TourLog>
                {
                    CreateValidTourLogEntity(1, tourId)
                };

                _mockTourLogRepository
                    .Setup(r => r.GetTourLogsByTourIDAsync(tourId))
                    .ReturnsAsync(logs);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() =>
                    _tourLogService.GetTourLogsAsync(tourId, userId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _tourLogService.GetTourLogsAsync(tourId, userId);
                var resultList = result.ToList();

                Assert.That(resultList.Count, Is.EqualTo(1));
                Assert.That(resultList.First().Comment, Is.EqualTo("Great log by Elmin"));
            }
        }
        // ------------------------------------------------------------
        // TESTING IF I CAN GET A SPECIFC TOUR LOG BY ID
        // ------------------------------------------------------------

        //log exists, tour exists == NORMAL OP
        [TestCase(true, true, false, null)]

        //log is already deleted and the tour exists == IMAGINE DELETING TWICE BACK TO BACK
        [TestCase(false, true, false, null)]

        //LOG EXISTS BUT AT THE ALMOST SAME MOMENT someone deleted the tour == TOUR NOT FOUND
        [TestCase(true, false, false, "Not Found: Tour not found. (BL13)")]
        //DB ERROR
        [TestCase(true, true, true, "Failed to retrieve tour log. (BL15)")]
        public async Task GetTourLogByTourLogIDAsync_Scenarios(
            bool logExists,
            bool tourExists,
            bool throwRepoException,
            string? expectedExceptionMsg)
        {
            int logId = 1, tourId = 1, userId = 1;

            var log = logExists ? CreateValidTourLogEntity(logId, tourId) : null;
            var tour = tourExists ? CreateValidTourEntity(tourId, userId) : null;

            if (throwRepoException)
            {
                _mockTourLogRepository
                    .Setup(r => r.GetTourLogByIDAsync(logId))
                    .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourLogRepository
                    .Setup(r => r.GetTourLogByIDAsync(logId))
                    .ReturnsAsync(log);
            }

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(tourId))
                .ReturnsAsync(tour);

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() =>
                    _tourLogService.GetTourLogByTourLogIDAsync(logId, userId));

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
                Assert.That(result.TourID, Is.EqualTo(tourId));
                Assert.That(result.Comment, Is.EqualTo("Great log by Elmin"));
            }
        }

        // ------------------------------------------------------------
        // ADD NEW TOUR LOG
        // ------------------------------------------------------------

        //TOUR EXISTS, USER IS VALID == NORMAL OP
        [TestCase(true, true, false, null)]

        //TOUR DOES NOT EXISTS == NORMAL OP
        [TestCase(false, true, false, "Not Found: Tour not found. (BL16)")]

        //USER DOES NOT EXISTS - SOMEONE IS CHEATING
        [TestCase(true, false, false, "Not Found: User does not exist. (BL16)")]

        // DB ERROR
        [TestCase(true, true, true, "Failed to add tour log. (BL18)")]
        public async Task AddTourLogAsync_Scenarios(
            bool tourExists,
            bool userExists,
            bool throwRepoException,
            string? expectedExceptionMsg)
        {
            int tourId = 1, userId = 1;

            var dto = CreateValidTourLogDto();
            var tour = tourExists ? CreateValidTourEntity(tourId, userId) : null;
            var user = userExists ? CreateValidUser(userId, "Elmin") : null;

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(tourId))
                .ReturnsAsync(tour);

            _mockUserRepository
                .Setup(r => r.GetUserByIDAsync(userId))
                .ReturnsAsync(user);

            // Used by RecalculateComputedAttributesAsync()
            _mockTourLogRepository
                .Setup(r => r.GetTourLogsByTourIDAsync(tourId))
                .ReturnsAsync(new List<TourLog>());

            if (throwRepoException)
            {
                _mockTourLogRepository
                    .Setup(r => r.AddTourLogAsync(It.IsAny<TourLog>()))
                    .ThrowsAsync(new DataAccessException("DB Error"));
            }
            else
            {
                _mockTourLogRepository
                    .Setup(r => r.AddTourLogAsync(It.IsAny<TourLog>()))
                    .Callback<TourLog>(l => l.log_id = 99)
                    .ReturnsAsync((TourLog l) => l);
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() =>
                    _tourLogService.AddTourLogAsync(dto, tourId, userId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));

                if (!throwRepoException)
                {
                    _mockTourLogRepository.Verify(
                        r => r.AddTourLogAsync(It.IsAny<TourLog>()),
                        Times.Never);
                }
            }
            else
            {
                var result = await _tourLogService.AddTourLogAsync(dto, tourId, userId);

                Assert.That(result, Is.Not.Null);
                Assert.That(result.ID, Is.EqualTo(99));
                Assert.That(result.Username, Is.EqualTo("Elmin"));

                _mockTourLogRepository.Verify(r =>
                        r.AddTourLogAsync(It.Is<TourLog>(l =>
                            l.Username == "Elmin" &&
                            l.comment == dto.Comment &&
                            l.difficulty == dto.Difficulty &&
                            l.totalDistance == dto.TotalDistance &&
                            l.totalTime == dto.TotalTime &&
                            l.rating == dto.Rating &&
                            l.logDateTime.Kind == DateTimeKind.Utc)),
                    Times.Once);

                // RecalculateComputedAttributesAsync() should update the tour
                _mockTourRepository.Verify(
                    r => r.UpdateTourAsync(It.IsAny<Tour>()),
                    Times.Once);
            }
        }
        // ------------------------------------------------------------
        // UPDATE LOG
        // ------------------------------------------------------------

        //tour EXISTS, user EXISTS, log EXISTS, CORRECT TOUR, OWNER OF LOG IS CORRECT == NORMAL OP
        [TestCase(true, true, true, true, true, false, null)]
        // TOUR NOT EXISTS
        [TestCase(false, true, true, true, true, false, "Not Found: Tour not found. (BL19)")]
        //CURRENT USER IS HACKING
        [TestCase(true, false, true, true, true, false,
            "Not Found: User does not exist (BL26)")]
        //LOG IS ALREADY GONE
        [TestCase(true, true, false, true, true, false,
            "Not Found: Tour log not found. (BL21)")]
        //SOMEONE IS TRYING TO ADD INVALID LOGS
        [TestCase(true, true, true, false, true, false,
            "Bad Request: Tour log does not belong to the specified tour. (BL22)")]
        //SOMEONE TRYING TO CHANGE LOGS WHICH DO NOT BELONG TO HIM
        [TestCase(true, true, true, true, false, false,
            "Forbidden: User has insufficient permissions to update a log for this tour. (BL20)")]
        //DB ERRROR
        [TestCase(true, true, true, true, true, true,
            "Failed to update tour log. (BL23)")]
        public async Task UpdateTourLogAsync_Scenarios(
            bool tourExists,
            bool userExists,
            bool logExists,
            bool correctTourId,
            bool usernameMatches,
            bool throwRepoException,
            string? expectedExceptionMsg)
        {
            int tourId = 1, logId = 1, userId = 1;

            var dto = CreateValidTourLogDto();

            var tour = tourExists
                ? CreateValidTourEntity(tourId, userId)
                : null;

            var existingLog = logExists
                ? CreateValidTourLogEntity(logId, correctTourId ? tourId : 999)
                : null;

            // IMPORTANT: ownership is based on username
            if (existingLog != null)
            {
                existingLog.Username = usernameMatches ? "Elmin" : "SomeoneElse";
            }

            var currentUser = userExists
                ? CreateValidUser(userId, "Elmin")
                : null;

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(tourId))
                .ReturnsAsync(tour);

            _mockTourLogRepository
                .Setup(r => r.GetTourLogByIDAsync(logId))
                .ReturnsAsync(existingLog);

            _mockUserRepository
                .Setup(r => r.GetUserByIDAsync(userId))
                .ReturnsAsync(currentUser);

            _mockTourLogRepository
                .Setup(r => r.GetTourLogsByTourIDAsync(tourId))
                .ReturnsAsync(new List<TourLog>());

            if (throwRepoException)
            {
                _mockTourLogRepository
                    .Setup(r => r.UpdateTourLogAsync(It.IsAny<TourLog>()))
                    .ThrowsAsync(new DataAccessException("DB Error"));
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() =>
                    _tourLogService.UpdateTourLogAsync(dto, tourId, logId, userId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));

                if (!throwRepoException)
                {
                    _mockTourLogRepository.Verify(
                        r => r.UpdateTourLogAsync(It.IsAny<TourLog>()),
                        Times.Never);
                }
            }
            else
            {
                await _tourLogService.UpdateTourLogAsync(dto, tourId, logId, userId);

                Assert.That(existingLog!.comment, Is.EqualTo(dto.Comment));
                Assert.That(existingLog.logDateTime.Kind, Is.EqualTo(DateTimeKind.Utc));

                _mockTourLogRepository.Verify(
                    r => r.UpdateTourLogAsync(existingLog),
                    Times.Once);

                _mockTourRepository.Verify(
                    r => r.UpdateTourAsync(tour!),
                    Times.Once);
            }
        }

        [TestCase(true, true, true, true, false, null)]
        [TestCase(false, true, true, true, false, "Not Found: Tour not found. (BL24)")]
        [TestCase(true, false, true, true, false,
            "Not Found: User does not exist (BL26)")]
        [TestCase(true, true, false, true, false, "Not Found: Tour log not found. (BL26)")]
        [TestCase(true, true, true, false, false,
            "Bad Request: Tour log does not belong to the specified tour. (BL27)")]
        public async Task DeleteTourLogAsync_Scenarios(
            bool tourExists,
            bool userExists,
            bool logExists,
            bool correctTourId,
            bool throwRepoException,
            string? expectedExceptionMsg)
        {
            int tourId = 1, logId = 1, userId = 1;

            var tour = tourExists
                ? CreateValidTourEntity(tourId, userId)
                : null;

            var existingLog = logExists
                ? CreateValidTourLogEntity(logId, correctTourId ? tourId : 999)
                : null;

            if (existingLog != null)
                existingLog.Username = userExists ? "Elmin" : "SomeoneElse";

            _mockTourRepository
                .Setup(r => r.GetTourByTourIDAsync(tourId))
                .ReturnsAsync(tour);

            _mockTourLogRepository
                .Setup(r => r.GetTourLogByIDAsync(logId))
                .ReturnsAsync(existingLog);

            _mockUserRepository
                .Setup(r => r.GetUserByIDAsync(userId))
                .ReturnsAsync(userExists ? CreateValidUser(userId, "Elmin") : null);

            _mockTourLogRepository
                .Setup(r => r.GetTourLogsByTourIDAsync(tourId))
                .ReturnsAsync(new List<TourLog>());

            if (throwRepoException)
            {
                _mockTourLogRepository
                    .Setup(r => r.DeleteTourLogAsync(logId))
                    .ThrowsAsync(new DataAccessException("DB Error"));
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<BusinessException>(() =>
                    _tourLogService.DeleteTourLogAsync(tourId, logId, userId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));

                _mockTourLogRepository.Verify(
                    r => r.DeleteTourLogAsync(It.IsAny<int>()),
                    Times.Never);

                return;
            }

            await _tourLogService.DeleteTourLogAsync(tourId, logId, userId);

            _mockTourLogRepository.Verify(
                r => r.DeleteTourLogAsync(logId),
                Times.Once);

            _mockTourRepository.Verify(
                r => r.UpdateTourAsync(tour!),
                Times.Once);
        }
    }
}