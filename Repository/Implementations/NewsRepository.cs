using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using MongoDB.Driver;
using webjooneli.Settings;


namespace webjooneli.Repository.Implementations
{
    public class NewsRepository : INewsRepository
    {
        private readonly ILogger<NewsRepository> _logger;
        private readonly IMongoCollection<NewsModel> _newscollection;
        private readonly GridFSBucket _gridFS;

        public NewsRepository(MongoDbContext dbContext, ILogger<NewsRepository> logger)
        {
            var collectionname = nameof(NewsModel).Replace("Model", "");
            _newscollection = dbContext.GetCollection<NewsModel>(collectionname);
            _logger = logger;
            _gridFS = dbContext.GridFsBucket;
        }
        public async Task CreateNewsAsync(NewsModel news, IFormFile imageFile)
        {
            try
            {
                _logger.LogInformation($"Creating news with title: {news.Title}");
                if (imageFile != null && imageFile.Length > 0)
                {
                    using var inputStream = imageFile.OpenReadStream();

                    // Load image using ImageSharp
                    using var image = await Image.LoadAsync(inputStream);
                    _logger.LogInformation("Image loaded successfully: {FileName}", imageFile.FileName);
                    // Optional: Resize/compress
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(1200, 800)
                    }));

                    // Save as WebP
                    using var webpStream = new MemoryStream();
                    await image.SaveAsWebpAsync(webpStream, new WebpEncoder
                    {
                        Quality = 75
                    });
                    _logger.LogInformation("Image converted to WebP format successfully: {FileName}", imageFile.FileName);
                    webpStream.Position = 0;

                    var fileId = await _gridFS.UploadFromStreamAsync(
                        $"{Path.GetFileNameWithoutExtension(imageFile.FileName)}.webp",
                        webpStream,
                        new GridFSUploadOptions
                        {
                            Metadata = new BsonDocument
                            {
                        { "ContentType", "image/webp" }
                            }
                        });

                    news.ImageId = fileId.ToString();
                }

                await _newscollection.InsertOneAsync(news);
                _logger.LogInformation("News with WebP image saved to GridFS.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving news");
                throw; // Re-throw or handle as needed
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

        public async Task UpdateNewsAsync(string id, NewsModel updatedNews)
        {
            try
            {
                var filter = Builders<NewsModel>.Filter.Eq(n => n.Id, id);
                var result = await _newscollection.ReplaceOneAsync(filter, updatedNews);

                if (result.ModifiedCount > 0)
                {
                    _logger.LogInformation("News updated successfully!");
                }
                else
                {
                    _logger.LogWarning("No news found to update with id: {Id}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update news.");
                throw;
            }
        }
    }
}
