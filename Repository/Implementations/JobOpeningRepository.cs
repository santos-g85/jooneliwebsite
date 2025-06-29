using MongoDB.Driver;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using webjooneli.Settings;

namespace webjooneli.Repository.Implementations
{
    public class JobOpeningRepository : IJobOpeningRepository
    {
        private readonly ILogger<JobOpeningRepository> _logger;
        private readonly IMongoCollection<JobOpeningModel> _jobOpeningsCollection;

        public JobOpeningRepository(MongoDbContext mongoDbContext, 
            ILogger<JobOpeningRepository> logger)
        {
            var collectionName = nameof(JobOpeningModel).Replace("Model", "");
            _jobOpeningsCollection = mongoDbContext.GetCollection<JobOpeningModel>(collectionName);
            _logger = logger;
        }

        public async Task<List<JobOpeningModel>> GetAllJobOpeningsAsync()
        {
            return await _jobOpeningsCollection.Find(_ => true).ToListAsync();  
        }

        public async Task<JobOpeningModel> GetJobOpeningByIdAsync(string id)
        {
            var filter = Builders<JobOpeningModel>.Filter.Eq(job => job.Id, id);
            return await _jobOpeningsCollection.Find(filter).FirstOrDefaultAsync();  
        }

        public async Task CreateJobOpeningAsync(JobOpeningModel jobOpening)
        {
            await _jobOpeningsCollection.InsertOneAsync(jobOpening);  
        }

        public async Task UpdateJobOpeningAsync(string id, JobOpeningModel jobOpening)
        {
            var filter = Builders<JobOpeningModel>.Filter.Eq(job => job.Id, id);
            var update = Builders<JobOpeningModel>.Update
                .Set(job => job.Title, jobOpening.Title)
                .Set(job => job.Description, jobOpening.Description)
                .Set(job => job.LogoUrl, jobOpening.LogoUrl)
                .Set(job => job.Department, jobOpening.Department)
                .Set(job => job.Location, jobOpening.Location)
                .Set(job => job.Company, jobOpening.Company)
                .Set(job => job.SalaryRange, jobOpening.SalaryRange)
                .Set(job => job.EmploymentType, jobOpening.EmploymentType)
                .Set(job => job.PostedAt, jobOpening.PostedAt)
                .Set(job => job.OpeningNo, jobOpening.OpeningNo)
                .Set(job => job.IsActive, jobOpening.IsActive);

            await _jobOpeningsCollection.UpdateOneAsync(filter, update);  
        }

        public async Task DeleteJobOpeningAsync(string id)
        {
            var filter = Builders<JobOpeningModel>.Filter.Eq(job => job.Id, id);
            await _jobOpeningsCollection.DeleteOneAsync(filter);  
        }
    }
}
