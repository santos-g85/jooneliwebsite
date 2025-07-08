using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using MongoDB.Driver;
using webjooneli.Settings;
using webjooneli.Services.Interfaces;


namespace webjooneli.Repository.Implementations
{
    public class NewsRepository : INewsRepository
    {
        private readonly ILogger<NewsRepository> _logger;
        private readonly IMongoCollection<NewsModel> _newscollection;
        private readonly IMongoCollection<NewsSubscriptionModel> _newsSubscription;
        private readonly IImageService _imageService;

        public NewsRepository(MongoDbContext dbContext, 
            ILogger<NewsRepository> logger,
            IImageService imageService)
        {
            var collectionname = nameof(NewsModel).Replace("Model", "");
            _newscollection = dbContext.GetCollection<NewsModel>(collectionname);

            var subsription = nameof(NewsSubscriptionModel).Replace("Model", "");
            _newsSubscription = dbContext.GetCollection<NewsSubscriptionModel>(subsription);

            _logger = logger;
            _imageService = imageService;
        }

        public async Task CreateNewsAsync(NewsModel news)
        {
            _logger.LogInformation("news arrived in news repo!");
            try
            {
                _logger.LogInformation($"Creating news with imageId: {news.ImageId}");
                await _newscollection.InsertOneAsync(news);
                _logger.LogInformation("Successfully created news!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
 
       public async Task DeleteNewsAsync(string id)
        {
            try {
                var filter = Builders<NewsModel>.Filter.Eq(n => n.Id, id);
                var result = await _newscollection.DeleteOneAsync(filter);
                if (result.DeletedCount > 0)
                {
                    _logger.LogInformation("News deleted successfully!");
                }
                else
                {
                    _logger.LogWarning("No news found with id: {Id}", id);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
           
        }

        public async Task<List<NewsModel>> GetAllNewsAsync()
        {
            try
            { 
                return await _newscollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve all news.");
                throw;
            }
        }

        public async Task<NewsModel> GetNewsByIdAsync(string id)
        {
            try
            {
                return await _newscollection.Find(n => n.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve news by ID.");
                throw;
            }
        }
        public async Task<List<NewsModel>> GetNewsByDateAsync()
        {
            try
            {
                return await _newscollection
                    .Find(_ => true)
                    .SortByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve news by date.");
                throw;
            }
        }

        public async Task UpdateNewsAsync( NewsModel updatedNews)
        {
           // var news = _newscollection.Find(x => x.Id , updatedNews.Id);
            try
            {
               
                var filter = Builders<NewsModel>.Filter.Eq(news => news.Id, updatedNews.Id);
                var update = Builders<NewsModel>.Update
                    .Set(news => news.Title, updatedNews.Title)
                    .Set(news => news.Content, updatedNews.Content)
                    .Set(news => news.Category, updatedNews.Category)
                    .Set(news=>news.IsFeatured, updatedNews.IsFeatured)
                    .Set(news => news.ImageId, updatedNews.ImageId)
                    .Set(news => news.Source,updatedNews.Source)
                    .Set(news => news.SourceUrl, updatedNews.SourceUrl);

                var result = await _newscollection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount > 0)
                {
                    _logger.LogInformation("News updated successfully!");
                }
                else
                {
                    _logger.LogWarning("No news found to update with id: {Id}", updatedNews.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update news.");
                throw;
            }

        }

        public async Task CreateSubscription(NewsSubscriptionModel usermail)
        {
            var filter = Builders<NewsSubscriptionModel>.Filter.Eq(e => e.Email, usermail.Email);
            var useremail = await _newsSubscription.Find(filter).FirstOrDefaultAsync();

            if (useremail == null)
            {
                var usermailinfo = new NewsSubscriptionModel
                {
                    Email = usermail.Email, 
                    Subscribed = DateTime.Now,
                };
                try
                {
                    await _newsSubscription.InsertOneAsync(usermailinfo);
                    _logger.LogInformation("Subscription created successfully!");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("Email already subscribed!");
            }
        }
    }
}


