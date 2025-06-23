using webjooneli.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace webjooneli.Repository.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplicationModel> GetApplicationByIdAsync(string id);
        Task<List<JobApplicationModel>> GetAllApplicationsAsync();
        Task CreateApplicationAsync(JobApplicationModel application);
        Task DeleteApplicationAsync(string id);
        //Task DropCV(CVUploadModel cvdrop, IFormFile cVFile);
        Task<JobOpeningModel> GetJobOpeningByJobOpeningIdAsync(string JobOpeningId);  // Fetch job opening by JobOpeningId of application
    }
}
