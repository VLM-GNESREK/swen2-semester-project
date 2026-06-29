using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Entities;

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
            return await _context.Tours.ToListAsync();
        }

        public async Task<Tour?> GetTourByIDAsync(int tourID)
        {
            return await _context.Tours.FirstOrDefaultAsync(t => t.tour_id == tourID);
        }

        public async Task<Tour> AddTourAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task UpdateTourAsync(Tour tour)
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTourAsync(int tourID)
        {
            var tour = await _context.Tours.FindAsync(tourID);
            if(tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
        }
    }
}