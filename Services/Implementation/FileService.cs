using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using webjooneli.Services.Interfaces;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private const string UploadsFolder = "uploads/files";
    public const long MaxFileSizeBytes = 20 * 1024 * 1024; 

    // Allowed MIME types and extensions
    private readonly Dictionary<string, string[]> _allowedFileTypes = new()
    {
        ["Documents"] = new[] { ".pdf", ".doc", ".docx", ".txt", ".rtf" },
        ["Spreadsheets"] = new[] { ".xls", ".xlsx", ".csv" },
        ["Presentations"] = new[] { ".ppt", ".pptx" },
        ["Text"] = new[] { ".txt", ".log", ".md" }
    };

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public async Task<FileUploadResult> UploadFileAsync(IFormFile file)
    {
        try
        {
            if (!IsFileValid(file))
            {
                return new FileUploadResult(
                    false,
                    null,
                    null,
                    null,
                    0,
                    "Invalid file type or size"
                );
            }

            // Create directory if it doesn't exist
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", UploadsFolder);
            Directory.CreateDirectory(uploadsPath);

            // Get file extension and generate safe filename
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var relativePath = Path.Combine(UploadsFolder, fileName);
            var fullPath = Path.Combine(uploadsPath, fileName);

            // Save the file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new FileUploadResult(
                true,
                relativePath.Replace("\\", "/"),
                file.FileName,
                file.ContentType,
                file.Length
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return new FileUploadResult(
                false,
                null,
                null,
                null,
                0,
                ex.Message
            );
        }
    }

    public bool DeleteFile(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                filePath.TrimStart('/')
            );

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return false;
        }
    }

    public bool IsFileValid(IFormFile file)
    {
        if (file == null || file.Length == 0 || file.Length > MaxFileSizeBytes)
            return false;

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        // Check if extension is in any of the allowed categories
        return _allowedFileTypes.Values
            .Any(extensions => extensions.Contains(fileExtension));
    }

    public async Task<FileDownloadResult> DownloadFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new FileDownloadResult(
                    false,
                    null,
                    null,
                    null,
                    "File path is empty"
                );
            }

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                filePath.TrimStart('/')
            );

            if (!File.Exists(fullPath))
            {
                return new FileDownloadResult(
                    false,
                    null,
                    null,
                    null,
                    "File not found"
                );
            }

            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            var contentType = GetContentType(fullPath);
            var fileName = Path.GetFileName(fullPath);

            return new FileDownloadResult(
                true,
                fileBytes,
                contentType,
                fileName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file");
            return new FileDownloadResult(
                false,
                null,
                null,
                null,
                ex.Message
            );
        }
    }

    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            _ => "application/octet-stream" // Default for unknown types
        };
    }
    public IEnumerable<string> GetAllowedExtensions()
    {
        return _allowedFileTypes.Values.SelectMany(x => x).Distinct();
    }

}