using webjooneli.Models.Entities;

namespace webjooneli.Repository.Interfaces
{
    public interface ICVUploadRepository
    {
        Task<CVUploadModel> GetCVByIdAsync(string id);

        Task<List<CVUploadModel>> GetAllCVsAsync();
        Task<string> CreateCVAsync(CVUploadModel cvUpload, IFormFile cvFile);
        Task DeleteCVAsync(string id);
    }
}
