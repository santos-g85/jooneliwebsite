using MongoDB.Driver;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using webjooneli.Settings;

namespace webjooneli.Repository.Implementations
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ILogger<SessionRepository> _logger;
        private readonly IMongoCollection<UserSessionsModel> _sessionCollection;

        public SessionRepository(ILogger<SessionRepository> logger, MongoDbContext dbContext)
        {
            _logger = logger;
            var collectionName = nameof(UserSessionsModel).Replace("Model", "");
            _sessionCollection = dbContext.GetCollection<UserSessionsModel>(collectionName);
        }

        public async Task CreateSessionAsync(UserSessionsModel session)
        {
            try
            {  
            await _sessionCollection.InsertOneAsync(session);
            _logger.LogInformation("Session created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating session.");
                throw;
            }
        }

        public async Task<UserSessionsModel> GetByIdAsync(string userId)
        {
            var filter = Builders<UserSessionsModel>.Filter.Eq(x => x.UserId, userId);
            return await _sessionCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateSessionAsync(string userId, UserSessionsModel session)
        {
            var filter = Builders<UserSessionsModel>.Filter.Eq(s => s.UserId, userId);
            var update = Builders<UserSessionsModel>.Update
                .Set(s => s.SessionToken, session.SessionToken)
                .Set(s => s.ExpiresAt, session.ExpiresAt)
                .Set(s => s.LastVisited, DateTime.UtcNow); 

            var result = await _sessionCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
                _logger.LogInformation("Session updated successfully.");
            else
                _logger.LogWarning("No session was updated (possibly not found).");
        }
    }
}
