using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Entities;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.DAL.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly TourPlannerDBContext _context;

        public TourRepository(TourPlannerDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tour>> GetAllToursAsync()
        {
            try
            {
                return await _context.Tours.ToListAsync();
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to fetch the tours. (DAL1)", ex);
            }
        }

        public async Task<Tour?> GetTourByTourIDAsync(int tourID)
        {
            try
            {
                return await _context.Tours.FirstOrDefaultAsync(t => t.tour_id == tourID);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to fetch the tour. (DAL2)", ex);
            }        
        }

        public async Task<IEnumerable<Tour>> GetToursByUserIDAsync(int userID)
        {
            try
            {
                return await _context.Tours.Where(t => t.user_id == userID).ToListAsync();
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred whilst trying to fetch the tour. (DAL2)", ex);
            }        
        }

        public async Task<Tour> AddTourAsync(Tour tour)
        {   
            try
            {
                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();
                return tour;
            }
            catch(DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred while creating the tour. (DAL3)", ex);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while creating the tour. (DAL4)", ex);
            }
        }

        public async Task UpdateTourAsync(Tour tour)
        {
            try
            {
                _context.Tours.Update(tour);
                await _context.SaveChangesAsync();  
            }
            catch(DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred while trying to update the tour. (DAL5)", ex);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while trying to update the tour. (DAL6)", ex);
            }
        }

        public async Task DeleteTourAsync(int tourID)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(tourID);
                if(tour != null)
                {
                    _context.Tours.Remove(tour);
                    await _context.SaveChangesAsync();
                }
            }
            catch(DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred while trying to delete the tour. (DAL7)", ex);
            }
            catch(Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred during the deletion of the tour. (DAL8)", ex);
            }
        }
    }
}