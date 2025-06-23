using MongoDB.Driver;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using webjooneli.Settings;

namespace webjooneli.Repository.Implementations
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly IMongoCollection<JobApplicationModel> _applicationCollection;
        private readonly ILogger<JobApplicationRepository> _logger;
        private readonly IJobOpeningRepository _jobOpeningCollection;

        public JobApplicationRepository(MongoDbContext mongoDbContext, ILogger<JobApplicationRepository> logger, IJobOpeningRepository jobOpeningRepository)
        {
            var applicationCollectionName = nameof(JobApplicationModel).Replace("Model", "");
            _applicationCollection = mongoDbContext.GetCollection<JobApplicationModel>(applicationCollectionName);

            _logger = logger;
            _jobOpeningCollection = jobOpeningRepository;
        }

        public async Task<JobApplicationModel> GetApplicationByIdAsync(string id)
        {
            var filter = Builders<JobApplicationModel>.Filter.Eq(a => a.Id, id);
            return await _applicationCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<JobApplicationModel>> GetAllApplicationsAsync()
        {
            return await _applicationCollection.Find(_ => true).ToListAsync();
        }

        public async Task CreateApplicationAsync(JobApplicationModel application)
        {
            // Validate the JobOpeningId exists
            var jobOpening = await _jobOpeningCollection.GetJobOpeningByIdAsync(application.JobOpeningId);
            if (jobOpening == null)
            {
                throw new Exception("The specified Job Opening does not exist.");
            }

            // Add the job application
            await _applicationCollection.InsertOneAsync(application);
        } 
       public async Task DeleteApplicationAsync(string id)
        {
            var filter = Builders<JobApplicationModel>.Filter.Eq(a => a.Id, id);
            await _applicationCollection.DeleteOneAsync(filter);
        }

        public async Task<JobOpeningModel> GetJobOpeningByJobOpeningIdAsync(string JobOpeningId)
        {
            // Fetch the application to get the JobOpeningId
            var application = await _applicationCollection.Find(a => a.Id == JobOpeningId).FirstOrDefaultAsync();

            if (application == null)
            {
                return null;
            }
            // Fetch the JobOpening using the JobOpeningId from the application
            var jobOpening = await _jobOpeningCollection.GetJobOpeningByIdAsync(application.JobOpeningId);
            return jobOpening;
        }
    }
}
