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

        public async Task<IEnumerable<TourLogDTO>> GetTourLogsAsync(int tourID, int userID)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourID);

                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL10)");
                }
                if(tour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to access this tour's logs. (BL11)");
                }

                var logs = await _tourLogRepository.GetTourLogsByTourIDAsync(tourID);

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

        public async Task<TourLogDTO?> GetTourLogByTourLogIDAsync(int logID, int userID)
        {
            try
            {
                var log = await _tourLogRepository.GetTourLogByIDAsync(logID);

                if(log == null)
                {
                    return null;
                }

                var tour = await _tourRepository.GetTourByTourIDAsync(log.tour_id);
                if(tour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to access this tour log. (BL13)");
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
                throw new BusinessException("Failed to retrieve tour log. (BL14)", ex);
            }
        }

        public async Task<TourLogDTO> AddTourLogAsync(TourLogDTO tourLog, int tourID, int userID)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourID);
                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL15)");
                }
                if(tour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to add a log to this tour. (BL16)");
                }

                var newLog = new TourLog
                {
                    tour_id = tourID,
                    logDateTime = tourLog.Date,
                    comment = tourLog.Comment,
                    difficulty = tourLog.Difficulty,
                    totalDistance = tourLog.TotalDistance,
                    totalTime = tourLog.TotalTime,
                    rating = tourLog.Rating
                };

                var createdLog = await _tourLogRepository.AddTourLogAsync(newLog);
                await RecalculateComputedAttributesAsync(tourID);

                tourLog.ID = createdLog.log_id;
                return tourLog;
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to add tour log. (BL17)", ex);
            }
        }

        public async Task UpdateTourLogAsync(TourLogDTO tourLog, int tourID, int tourLogID, int userID)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourID);
                var existingLog = await _tourLogRepository.GetTourLogByIDAsync(tourLogID);

                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL18)");
                }
                if(tour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to update a log for this tour. (BL19)");
                }
                if(existingLog == null)
                {
                    throw new BusinessException("Not Found: Tour log not found. (BL20)");
                }
                if(existingLog.tour_id != tourID)
                {
                    throw new BusinessException("Bad Request: Tour log does not belong to the specified tour. (BL21)");
                }

                existingLog.logDateTime = tourLog.Date;
                existingLog.comment = tourLog.Comment;
                existingLog.difficulty = tourLog.Difficulty;
                existingLog.totalDistance = tourLog.TotalDistance;
                existingLog.totalTime = tourLog.TotalTime;
                existingLog.rating = tourLog.Rating;

                await _tourLogRepository.UpdateTourLogAsync(existingLog);
                await RecalculateComputedAttributesAsync(tourID);
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to update tour log. (BL22)", ex);
            }
        }

        public async Task DeleteTourLogAsync(int tourID, int tourLogID, int userID)
        {
            try
            {
                var tour = await _tourRepository.GetTourByTourIDAsync(tourID);
                var existingLog = await _tourLogRepository.GetTourLogByIDAsync(tourLogID);

                if(tour == null)
                {
                    throw new BusinessException("Not Found: Tour not found. (BL23)");
                }
                if(tour.user_id != userID)
                {
                    throw new BusinessException("Unauthorised: User has insufficient permissions to delete a log for this tour. (BL24)");
                }
                if(existingLog == null)
                {
                    throw new BusinessException("Not Found: Tour log not found. (BL25)");
                }
                if(existingLog.tour_id != tourID)
                {
                    throw new BusinessException("Bad Request: Tour log does not belong to the specified tour. (BL26)");
                }

                await _tourLogRepository.DeleteTourLogAsync(tourLogID);
                await RecalculateComputedAttributesAsync(tourID);
            }
            catch(DataAccessException ex)
            {
                throw new BusinessException("Failed to delete tour log. (BL27)", ex);
            }
        }

        private async Task RecalculateComputedAttributesAsync(int tourID) // helper method to calc popularity/childfriendliness
        {
            var tour = await _tourRepository.GetTourByTourIDAsync(tourID);
            var logs = await _tourLogRepository.GetTourLogsByTourIDAsync(tourID);

            if(tour == null)
            {
                throw new BusinessException("Not Found: Tour not found, how did you even get here? (BL28)");
            }

            tour.popularity = logs.Count();
            
            const double MaxChildFriendlyDistance = 10000;
            const int MaxChildFriendlyTime = 10800;

            bool isChildFriendly = tour.distance <= MaxChildFriendlyDistance && tour.estimatedTime <= MaxChildFriendlyTime;

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