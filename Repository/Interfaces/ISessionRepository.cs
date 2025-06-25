using webjooneli.Models.Entities;

namespace webjooneli.Repository.Interfaces
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(UserSessionsModel session);

        Task UpdateSessionAsync(string UserId, UserSessionsModel session);

        Task<UserSessionsModel> GetByIdAsync(string userId);
    }
}
