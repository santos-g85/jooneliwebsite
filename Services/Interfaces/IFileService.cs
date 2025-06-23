namespace webjooneli.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string userName);
        Task<byte[]> DownloadFileAsync(string fileId);
    }
}
