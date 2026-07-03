using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Entities;
using TourPlanner.DAL.Exceptions;

namespace TourPlanner.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TourPlannerDBContext _context;

        public UserRepository(TourPlannerDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while fetching users. (DAL17)", ex);
            }
        }

        public async Task<User?> GetUserByIDAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.user_id == userId);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while fetching the user. (DAL18)", ex);
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                // This is the critical method the BL will call during the login process
                return await _context.Users.FirstOrDefaultAsync(u => u.username == username);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while fetching the user by username. (DAL19)", ex);
            }
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred while creating the user. (DAL20)", ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while creating the user. (DAL21)", ex);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred while updating the user. (DAL22)", ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred while updating the user. (DAL23)", ex);
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new DataAccessException("A database error occurred while deleting the user. (DAL24)", ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An unexpected error occurred during the deletion of the user. (DAL25)", ex);
            }
        }
    }
}