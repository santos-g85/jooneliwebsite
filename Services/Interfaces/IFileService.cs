namespace webjooneli.Services.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file);
        bool DeleteFile(string filePath);
        bool IsFileValid(IFormFile file);
        Task<FileDownloadResult> DownloadFileAsync(string filePath);
    }
    public record FileUploadResult(
    bool Success,
    string FilePath,
    string FileName,
    string FileType,
    long FileSize,
    string ErrorMessage = null
    );
    public record FileDownloadResult(
    bool Success,
    byte[] FileContent,
    string ContentType,
    string FileName,
    string ErrorMessage = null
    );
}
