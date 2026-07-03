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
                Username = "Elmin",
                comment = $"Log {logId} for Tour {tourId}",
                difficulty = 3,
                totalDistance = 11,
                totalTime = 120,
                rating = 4,
                logDateTime = DateTime.UtcNow
            };
        }

        // ------------------------------------------------------------
        // GET BY ID
        // ------------------------------------------------------------
        //ID =1 || doesDataExist in DB = true || break the context? = false, exepected Exception = null(so nothing)
        [TestCase(1, true, false, null)]
        
        //ID =1 || doesDataExist in DB = no || break the context? = false, exepected Exception = null(so nothing)
        [TestCase(1, false, false, null)]
        
        //ID =1 || doesDataExist in DB = yes || break the context? = YES, exepected Exception = read below
        [TestCase(1, true, true, "An unexpected error occurred whilst trying to fetch the Tour Log. (DAL9)")]
        public async Task GetTourLogByIDAsync_Scenarios(
            int targetLogId,
            bool usePredefinedData,
            bool forceException,
            string? expectedExceptionMsg)
        {
            if (usePredefinedData)
            {
                _context.TourLogs.Add(CreateSampleLog(targetLogId, 100));
                await _context.SaveChangesAsync();
            }

            if (forceException)
                _context.Dispose();

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() =>
                    _repository.GetTourLogByIDAsync(targetLogId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = await _repository.GetTourLogByIDAsync(targetLogId);

                if (usePredefinedData)
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

        // ------------------------------------------------------------
        // TESTING IF WE CAN GET A TOUR BY ID
        // ------------------------------------------------------------
        // 2 tour logs, dont break application, no exception == NORMAL OPERATION
        [TestCase(2, false, null)]
        
        //0 logs, dont break app, no exception == NORMAL OPERATION NO ONE CREATED LOGS YET
        [TestCase(0, false, null)]
        
        //2 tour logs, BREAAK THE CONTEXT, ERROR == SUDDEN DB OUTAGE
        [TestCase(2, true, "An unexpected error occurred whilst trying to fetch Tour Logs. (DAL10)")]
        public async Task GetTourLogsByTourIDAsync_Scenarios(
            int amountOfSampleLogs,
            bool forceException,
            string? expectedExceptionMsg)
        {
            int targetTourId = 100;

            for (int i = 1; i <= amountOfSampleLogs; i++)
                _context.TourLogs.Add(CreateSampleLog(i, targetTourId));

            _context.TourLogs.Add(CreateSampleLog(99, 200)); // noise data
            await _context.SaveChangesAsync();

            if (forceException)
                _context.Dispose();

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() =>
                    _repository.GetTourLogsByTourIDAsync(targetTourId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                var result = (await _repository.GetTourLogsByTourIDAsync(targetTourId)).ToList();

                Assert.That(result.Count, Is.EqualTo(amountOfSampleLogs));
                Assert.That(result.All(l => l.tour_id == targetTourId), Is.True);
            }
        }

        // ------------------------------------------------------------
        // TESTING ADDTOURLOG 
        // ------------------------------------------------------------
        [TestCase(false, null)]
        [TestCase(true, "An unexpected error occurred whilst trying to create the Tour Log. (DAL12)")]
        public async Task AddTourLogAsync_Scenarios(
            bool forceException,
            string? expectedExceptionMsg)
        {
            var newLog = CreateSampleLog(1, 100);

            if (forceException)
                _context.Dispose();

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() =>
                    _repository.AddTourLogAsync(newLog));

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

        // ------------------------------------------------------------
        // TESTING IF UPDATING WORKS
        // ------------------------------------------------------------
        [TestCase(false, null)]
        [TestCase(true, "An unexpected error occurred whilst trying to update the Tour Log. (DAL14)")]
        public async Task UpdateTourLogAsync_Scenarios(
            bool forceException,
            string? expectedExceptionMsg)
        {
            var existingLog = CreateSampleLog(1, 100);
            _context.TourLogs.Add(existingLog);
            await _context.SaveChangesAsync();

            _context.Entry(existingLog).State = EntityState.Detached;

            var updatedLog = CreateSampleLog(1, 100);
            updatedLog.comment = "Modified Comment";
            updatedLog.difficulty = 5;

            if (forceException)
                _context.Dispose();

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() =>
                    _repository.UpdateTourLogAsync(updatedLog));

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

        // ------------------------------------------------------------
        // TESTING IF DELETE TOUR LOG WORKS
        // ------------------------------------------------------------
        //the log exists, no exceptions == NORMAL OPERARTION
        [TestCase(true, false, null)]
        
        //LOG NOT EXSISTS, no exceptions == NORMAL OPERARTION
        [TestCase(false, false, null)]
        
        //log exists, but context is broken
        [TestCase(true, true, "An unexpected error occurred whilst trying to delete the Tour Log. (DAL16)")]
        public async Task DeleteTourLogAsync_Scenarios(
            bool logExists,
            bool forceException,
            string? expectedExceptionMsg)
        {
            int targetLogId = 1;

            if (logExists)
            {
                _context.TourLogs.Add(CreateSampleLog(targetLogId, 100));
                await _context.SaveChangesAsync();
            }

            if (forceException)
                _context.Dispose();

            if (expectedExceptionMsg != null)
            {
                var ex = Assert.ThrowsAsync<DataAccessException>(() =>
                    _repository.DeleteTourLogAsync(targetLogId));

                Assert.That(ex.Message, Is.EqualTo(expectedExceptionMsg));
            }
            else
            {
                await _repository.DeleteTourLogAsync(targetLogId);

                var dbLog = await _context.TourLogs.FirstOrDefaultAsync(l => l.log_id == targetLogId);

                Assert.That(dbLog, Is.Null);
            }
        }
    }
}