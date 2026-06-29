using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Entities;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.DAL.Repositories
{
    public class TourLogRepository : ITourLogRepository
    {
        private readonly TourPlannerDBContext _context;
        
        public TourLogRepository(TourPlannerDBContext context)
        {
            _context = context;
        }

        public async Task<TourLog?> GetTourLogByIDAsync(int logID)
        {
            try
            {
                return await _context.TourLogs.FirstOrDefaultAsync(tl => tl.log_id == logID); 
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to fetch the Tour Log. (DAL9)", ex);
            }
        }

        public async Task<IEnumerable<TourLog>> GetTourLogsByTourIDAsync(int tourID)
        {
            try
            {
                return await _context.TourLogs.Where(tl => tl.tour_id == tourID).ToListAsync();
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to fetch Tour Logs. (DAL10)", ex);
            }
        }

        public async Task<TourLog> AddTourLogAsync(TourLog tourLog)
        {
            try
            {
                _context.TourLogs.Add(tourLog);
                await _context.SaveChangesAsync();
                return tourLog;
            }
            catch(DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred whilst attempting to create the Tour Log. (DAL11)", ex);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to create the Tour Log. (DAL12)", ex);
            }
        }

        public async Task UpdateTourLogAsync(TourLog tourLog)
        {
            try
            {
                _context.TourLogs.Update(tourLog);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred whilst trying to update the Tour Log. (DAL13)", ex);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to update the Tour Log. (DAL14)", ex);
            }
        }

        public async Task DeleteTourLogAsync(int logID)
        {
            try
            {
                var tourLog = await _context.TourLogs.FindAsync(logID);
                if(tourLog != null)
                {
                    _context.TourLogs.Remove(tourLog);
                    await _context.SaveChangesAsync();
                }
            }
            catch(DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred whilst trying to delete the Tour Log. (DAL15)", ex);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to delete the Tour Log. (DAL16)", ex);
            }
        }
    }
}