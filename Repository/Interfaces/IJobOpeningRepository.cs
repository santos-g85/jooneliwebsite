using webjooneli.Models.Entities;

namespace webjooneli.Repository.Interfaces
{
    public interface IJobOpeningRepository
    {
        Task<List<JobOpeningModel>> GetAllJobOpeningsAsync();  
        Task<JobOpeningModel> GetJobOpeningByIdAsync(string id);  
        Task CreateJobOpeningAsync(JobOpeningModel jobOpening); 
        Task UpdateJobOpeningAsync(string id, JobOpeningModel jobOpening);  
        Task DeleteJobOpeningAsync(string id);  
    }
}
