using MongoDB.Driver;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using webjooneli.Settings;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace webjooneli.Repository.Implementations
{
    public class CVUploadRepository : ICVUploadRepository
    {
        private readonly IMongoCollection<CVUploadModel> _cvCollection;
        private readonly ILogger<CVUploadRepository> _logger;

        public CVUploadRepository(MongoDbContext mongoDbContext, ILogger<CVUploadRepository> logger)
        {
            var collectionName = nameof(CVUploadModel).Replace("Model", "");
            _cvCollection = mongoDbContext.GetCollection<CVUploadModel>(collectionName);
            _logger = logger;
        }

        // Method to get a CV by its ID
        public async Task<CVUploadModel> GetCVByIdAsync(string id)
        {
            var filter = Builders<CVUploadModel>.Filter.Eq(cv => cv.Id, id);
            return await _cvCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Method to create a new CV record
        public async Task CreateCVAsync(CVUploadModel cvUpload)
        {
            await _cvCollection.InsertOneAsync(cvUpload);
            _logger.LogInformation("CV uploaded successfully: {FullName}, FileId: {FileId}", cvUpload.Name, cvUpload.CVFileId);
        }

       
        // Method to delete a CV by its ID
        public async Task DeleteCVAsync(string id)
        {
            var filter = Builders<CVUploadModel>.Filter.Eq(cv => cv.Id, id);

            // Check if the filter is null or empty
            if (filter == null)
            {
                _logger.LogWarning("Attempted to delete a CV with a null ID.");
                return;
            }
            var result = await _cvCollection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                _logger.LogWarning("No CV found with the ID: {CVId}", id);
            }
            else
            {
                _logger.LogInformation("CV deleted successfully: {CVId}", id);
            }
        }

        public async Task<List<CVUploadModel>> GetAllCVsAsync()
        {
            return await _cvCollection.Find(_ => true).ToListAsync();
        }
    }
}
