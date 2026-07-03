using TourPlanner.BL.Interfaces;
using TourPlanner.DAL.Repositories;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Exceptions;
using TourPlanner.DAL.Entities;

namespace TourPlanner.BL.Services
{
    public class TourLogService : ITourLogService
    {
        private readonly ITourLogRepository _tourLogRepository;
        private readonly ITourRepository _tourRepository;

        public TourLogService(ITourLogRepository tourLogRepository, ITourRepository tourRepository)
        {
            _tourLogRepository = tourLogRepository;
            _tourRepository = tourRepository;
        }

        public async Task<IEnumerable<TourLogDTO>> GetTourLogsAsync(int tourId, int userId)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourId);

                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL10)");
                }
                if(tour.user_id != userId)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to access this tour's logs. (BL11)");
                }

                var logs = await _tourLogRepository.GetTourLogsByTourIDAsync(tourId);

                return logs.Select(log => new TourLogDTO
                {
                    ID = log.log_id,
                    TourID = log.tour_id,
                    Date = log.logDateTime,
                    Comment = log.comment ?? string.Empty,
                    Difficulty = log.difficulty,
                    TotalTime = log.totalTime,
                    Rating = log.rating
                }).ToList();
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to retrieve tour logs. (BL12)", ex);
            }
        }

        public async Task<TourLogDTO?> GetTourLogByTourLogIDAsync(int logId, int userId)
        {
            try
            {
                var log = await _tourLogRepository.GetTourLogByIDAsync(logId);

                if(log == null)
                {
                    return null;
                }

                var tour = await _tourRepository.GetTourByTourIDAsync(log.tour_id);
                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL13)");
                }
                if(tour.user_id != userId)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to access this tour log. (BL14)");
                }

                return new TourLogDTO
                {
                    ID = log.log_id,
                    TourID = log.tour_id,
                    Date = log.logDateTime,
                    Comment = log.comment ?? string.Empty,
                    Difficulty = log.difficulty,
                    TotalTime = log.totalTime,
                    Rating = log.rating
                };
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to retrieve tour log. (BL15)", ex);
            }
        }

        public async Task<TourLogDTO> AddTourLogAsync(TourLogDTO tourLog, int tourId, int userId)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourId);
                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL16)");
                }
                if(tour.user_id != userId)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to add a log to this tour. (BL17)");
                }

                var newLog = new TourLog
                {
                    tour_id = tourId,
                    logDateTime = tourLog.Date,
                    comment = tourLog.Comment,
                    difficulty = tourLog.Difficulty,
                    totalDistance = tourLog.TotalDistance,
                    totalTime = tourLog.TotalTime,
                    rating = tourLog.Rating
                };

                var createdLog = await _tourLogRepository.AddTourLogAsync(newLog);
                await RecalculateComputedAttributesAsync(tourId);

                tourLog.ID = createdLog.log_id;
                return tourLog;
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to add tour log. (BL18)", ex);
            }
        }

        public async Task UpdateTourLogAsync(TourLogDTO tourLog, int tourId, int tourLogId, int userId)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourId);
                var existingLog = await _tourLogRepository.GetTourLogByIDAsync(tourLogId);

                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL19)");
                }
                if(tour.user_id != userId)
                {
                    throw new BusinessException("Forbidden: User has insufficient permissions to update a log for this tour. (BL20)");
                }
                if(existingLog == null)
                {
                    throw new BusinessException("Not Found: Tour log not found. (BL21)");
                }
                if(existingLog.tour_id != tourId)
                {
                    throw new BusinessException("Bad Request: Tour log does not belong to the specified tour. (BL22)");
                }

                existingLog.logDateTime = tourLog.Date;
                existingLog.comment = tourLog.Comment;
                existingLog.difficulty = tourLog.Difficulty;
                existingLog.totalDistance = tourLog.TotalDistance;
                existingLog.totalTime = tourLog.TotalTime;
                existingLog.rating = tourLog.Rating;

                await _tourLogRepository.UpdateTourLogAsync(existingLog);
                await RecalculateComputedAttributesAsync(tourId);
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to update tour log. (BL23)", ex);
            }
        }

        public async Task DeleteTourLogAsync(int tourId, int tourLogId, int userId)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourId);
                var existingLog = await _tourLogRepository.GetTourLogByIDAsync(tourLogId);

                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL24)");
                }
                if(tour.user_id != userId)
                {
                    throw new BusinessException("Forbidden: User has insufficient permissions to delete a log for this tour. (BL25)");
                }
                if(existingLog == null)
                {
                    throw new BusinessException("Not Found: Tour log not found. (BL26)");
                }
                if(existingLog.tour_id != tourId)
                {
                    throw new BusinessException("Bad Request: Tour log does not belong to the specified tour. (BL27)");
                }

                await _tourLogRepository.DeleteTourLogAsync(tourLogId);
                await RecalculateComputedAttributesAsync(tourId);
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to delete tour log. (BL28)", ex);
            }
        }

        private async Task RecalculateComputedAttributesAsync(int tourId) // helper method to calc popularity/childfriendliness
        {
            var tour = await _tourRepository.GetTourByTourIDAsync(tourId);
            var logs = await _tourLogRepository.GetTourLogsByTourIDAsync(tourId);

            if(tour == null)
            {
                throw new BusinessException("Not Found: Tour not found, how did you even get here? (BL29)");
            }

            tour.popularity = logs.Count();
            
            const decimal maxChildFriendlyDistance = 10000;
            const int maxChildFriendlyTime = 10800;

            bool isChildFriendly = tour.distance <= maxChildFriendlyDistance && tour.estimatedTime <= maxChildFriendlyTime;

            if(logs.Any())
            {
                double averageDifficulty = logs.Average(l => l.difficulty);
                if(averageDifficulty > 2)
                {
                    isChildFriendly = false;
                }
            }

            tour.childFriendly = isChildFriendly ? 1 : 0;

            await _tourRepository.UpdateTourAsync(tour);
        }
    }
}