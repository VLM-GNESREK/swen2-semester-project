using TourPlanner.DAL.Entities;

namespace TourPlanner.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIDAsync(int userID);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userID);
    }
}