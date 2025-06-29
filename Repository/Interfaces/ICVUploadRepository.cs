using webjooneli.Models.Entities;

namespace webjooneli.Repository.Interfaces
{
    public interface ICVUploadRepository
    {
        Task<CVUploadModel> GetCVByIdAsync(string id);

        Task<List<CVUploadModel>> GetAllCVsAsync();
        Task CreateCVAsync(CVUploadModel cvUpload);
        Task DeleteCVAsync(string id);
    }
}
