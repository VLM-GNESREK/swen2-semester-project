using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourPlanner.DAL;
using TourPlanner.DAL.Entities;
using TourPlanner.DAL.Repositories;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.Tests.DAL
{
    [TestFixture]
    public class TourLogRepositoryTests
    {
        private DbContextOptions<TourPlannerDBContext> _options;
        private TourPlannerDBContext _context;
        private TourLogRepository _repository;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<TourPlannerDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TourPlannerDBContext(_options);
            _repository = new TourLogRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private TourLog CreateSampleLog(int logId, int tourId)
        {
            return new TourLog
            {
                log_id = logId,
                tour_id = tourId,
                comment = $"Log {logId} for Tour {tourId}",
                difficulty = 3,
                totalDistance = 10.5,
                totalTime = 120,
                rating = 4,
                logDateTime = DateTime.UtcNow
            };
        }

        [TestCase(1, true, false, (string?)null)]
        [TestCase(1, false, false, (string?)null)]
        [TestCase(0, false, false, (string?)null)]
        [TestCase(-1, false, false, (string?)null)]
        [TestCase(1, true, true, "An unexpected error occurred whilst trying to fetch the Tour Log. (DAL9)")]
        public async Task GetTourLogByIDAsync_Scenarios(int targetLogId, bool seedData, bool forceException, string? expectedExceptionMsg)
        {
            if (seedData)
            {
                _context.TourLogs.Add(CreateSampleLog(targetLogId, 100));
                await _context.SaveChangesAsync();
            }

            if (forceException)
            {
                _context.Dispose();
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() => _repository.GetTourLogByIDAsync(targetLogId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _repository.GetTourLogByIDAsync(targetLogId);
                
                if (seedData)
                {
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result!.log_id, Is.EqualTo(targetLogId));
                }
                else
                {
                    Assert.That(result, Is.Null);
                }
            }
        }

        [TestCase(2, false, (string?)null)]
        [TestCase(0, false, (string?)null)]
        [TestCase(2, true, "An unexpected error occurred whilst trying to fetch Tour Logs. (DAL10)")]
        public async Task GetTourLogsByTourIDAsync_Scenarios(int logsToSeed, bool forceException, string? expectedExceptionMsg)
        {
            int targetTourId = 100;
            for (int i = 1; i <= logsToSeed; i++)
            {
                _context.TourLogs.Add(CreateSampleLog(i, targetTourId));
            }
            _context.TourLogs.Add(CreateSampleLog(99, 200)); 
            await _context.SaveChangesAsync();

            if (forceException)
            {
                _context.Dispose();
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() => _repository.GetTourLogsByTourIDAsync(targetTourId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _repository.GetTourLogsByTourIDAsync(targetTourId);
                var resultList = result.ToList();

                Assert.That(resultList.Count, Is.EqualTo(logsToSeed));
                Assert.That(resultList.All(l => l.tour_id == targetTourId), Is.True);
            }
        }

        [TestCase(false, (string?)null)]
        [TestCase(true, "An unexpected error occurred whilst trying to create the Tour Log. (DAL12)")]
        public async Task AddTourLogAsync_Scenarios(bool forceException, string? expectedExceptionMsg)
        {
            var newLog = CreateSampleLog(1, 100);

            if (forceException)
            {
                _context.Dispose();
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() => _repository.AddTourLogAsync(newLog));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _repository.AddTourLogAsync(newLog);

                Assert.That(result, Is.Not.Null);

                var dbLog = await _context.TourLogs.FirstOrDefaultAsync(l => l.log_id == 1);
                Assert.That(dbLog, Is.Not.Null);
                Assert.That(dbLog!.comment, Is.EqualTo(newLog.comment));
            }
        }

        [TestCase(false, (string?)null)]
        [TestCase(true, "An unexpected error occurred whilst trying to update the Tour Log. (DAL14)")]
        public async Task UpdateTourLogAsync_Scenarios(bool forceException, string? expectedExceptionMsg)
        {
            var existingLog = CreateSampleLog(1, 100);
            _context.TourLogs.Add(existingLog);
            await _context.SaveChangesAsync();

            _context.Entry(existingLog).State = EntityState.Detached;

            var updatedLog = CreateSampleLog(1, 100);
            updatedLog.comment = "Modified Comment";
            updatedLog.difficulty = 5;

            if (forceException)
            {
                _context.Dispose();
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() => _repository.UpdateTourLogAsync(updatedLog));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                await _repository.UpdateTourLogAsync(updatedLog);

                var dbLog = await _context.TourLogs.FirstOrDefaultAsync(l => l.log_id == 1);
                Assert.That(dbLog, Is.Not.Null);
                Assert.That(dbLog!.comment, Is.EqualTo("Modified Comment"));
                Assert.That(dbLog.difficulty, Is.EqualTo(5));
            }
        }

        [TestCase(true, false, (string?)null)]
        [TestCase(false, false, (string?)null)]
        [TestCase(true, true, "An unexpected error occurred whilst trying to delete the Tour Log. (DAL16)")]
        public async Task DeleteTourLogAsync_Scenarios(bool logExists, bool forceException, string? expectedExceptionMsg)
        {
            int targetLogId = 1;
            if (logExists)
            {
                _context.TourLogs.Add(CreateSampleLog(targetLogId, 100));
                await _context.SaveChangesAsync();
            }

            if (forceException)
            {
                _context.Dispose();
            }

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() => _repository.DeleteTourLogAsync(targetLogId));
                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                await _repository.DeleteTourLogAsync(targetLogId);

                if (logExists)
                {
                    var dbLog = await _context.TourLogs.FirstOrDefaultAsync(l => l.log_id == targetLogId);
                    Assert.That(dbLog, Is.Null);
                }
            }
        }
    }
}