namespace webjooneli.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        Task<byte[]> DownloadImageAsync(string fileId);
    }
}
