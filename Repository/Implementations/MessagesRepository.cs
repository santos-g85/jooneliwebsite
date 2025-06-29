using webjooneli.Models.Entities;
using webjooneli.Settings;
using MongoDB.Driver;
using webjooneli.Repository.Interfaces;

namespace webjooneli.Repository.Implementations
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly ILogger<MessagesRepository> _logger;
        private readonly IMongoCollection<MessagesModel> _messagescollection;

        public MessagesRepository(MongoDbContext mongoDbContext, 
            ILogger<MessagesRepository> logger)
        {
            var collectionName = nameof(MessagesModel).Replace("Model", "");
            _messagescollection = mongoDbContext.GetCollection<MessagesModel>(collectionName);
            _logger = logger;
        }

        // Create a new message
        public async Task CreateMessageAsync(MessagesModel messages)
        {
            try
            {
                await _messagescollection.InsertOneAsync(messages);
                _logger.LogInformation("Message created successfully: {Name}", messages.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message");
                throw new Exception("Error creating message", ex);
            }
        }

        // Delete a message by its ID
        public async Task DeleteMessagesAsync(string id)
        {
            try
            {
                var filter = Builders<MessagesModel>.Filter.Eq(m => m.Id, id);
                await _messagescollection.DeleteOneAsync(filter);
                _logger.LogInformation("Message with Id {Id} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message with Id {Id}.", id);
                throw new Exception("Error deleting message", ex);
            }
        }

        // Get all messages
        public async Task<List<MessagesModel>> GetAllMessagesAsync()
        {
            try
            {
                return await _messagescollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all messages");
                throw new Exception("Error retrieving all messages", ex);
            }
        }

        // Get a message by its ID
        public async Task<MessagesModel> GetMessagesByIdAsync(string id)
        {
            try
            {
                var filter = Builders<MessagesModel>.Filter.Eq(m => m.Id, id);
                var message = await _messagescollection.Find(filter).FirstOrDefaultAsync();

                if (message == null)
                {
                    _logger.LogWarning("Message with Id {Id} not found.", id);
                }

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving message with Id {Id}.", id);
                throw new Exception("Error retrieving message by Id", ex);
            }
        }

        // Get all messages sorted by their creation date
        public async Task<List<MessagesModel>> GetAllMessagesSortedByDateAsync()
        {
            try
            {
                return await _messagescollection.Find(_ => true)
                    .SortByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching messages sorted by date.");
                throw new Exception("Error retrieving messages sorted by date", ex);
            }
        }
    }
}
