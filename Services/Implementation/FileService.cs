using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using webjooneli.Services.Interfaces;
using webjooneli.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace webjooneli.Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<FileService> _logger;

        public FileService(MongoDbContext mongoDbContext, ILogger<FileService> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }

        // Method to upload a file to GridFS
        public async Task<string> UploadFileAsync(IFormFile file, string userName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file selected for upload");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    // Metadata to be associated with the file
                    var metadata = new BsonDocument
                    {
                        { "UserName", userName },
                        { "UploadedAt", DateTime.UtcNow }
                    };

                    // Upload the file to GridFS and get the file ID
                    var fileId = await _mongoDbContext.GridFsBucket.UploadFromStreamAsync(file.FileName, stream, new GridFSUploadOptions
                    {
                        Metadata = metadata
                    });

                    _logger.LogInformation("File uploaded successfully: {FileName} with FileId: {FileId}", file.FileName, fileId.ToString());

                    return fileId.ToString(); // Return the file ID as a string
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error uploading file: {Error}", ex.Message);
                throw new Exception("Error uploading file to server", ex);
            }
        }

        // Method to download a file from GridFS
        public async Task<byte[]> DownloadFileAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("Invalid file ID");

            try
            {
                var objectId = new ObjectId(fileId); // Convert the string fileId to ObjectId

                // Download the file's data from GridFS
                using (var stream = await _mongoDbContext.GridFsBucket.OpenDownloadStreamAsync(objectId))
                {
                    var fileBytes = new byte[stream.Length];
                    await stream.ReadAsync(fileBytes, 0, (int)stream.Length);
                    return fileBytes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error downloading file: {Error}", ex.Message);
                throw new Exception("Error downloading file from server", ex);
            }
        }

        // Method to get metadata of a file from GridFS
        public async Task<BsonDocument> GetFileMetadataAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("Invalid file ID");

            try
            {
                var objectId = new ObjectId(fileId); // Convert the string fileId to ObjectId

                // Fetch file metadata from GridFS
                var fileInfo = await _mongoDbContext.GridFsBucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq(f => f.Id, objectId));
                var file = await fileInfo.FirstOrDefaultAsync();

                return file?.Metadata ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching file metadata: {Error}", ex.Message);
                throw new Exception("Error fetching file metadata from server", ex);
            }
        }

        // Method to delete a file from GridFS
        public async Task DeleteFileAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("Invalid file ID");

            try
            {
                var objectId = new ObjectId(fileId); // Convert the string fileId to ObjectId

                // Delete the file from GridFS
                await _mongoDbContext.GridFsBucket.DeleteAsync(objectId);

                _logger.LogInformation("File deleted successfully with FileId: {FileId}", fileId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting file: {Error}", ex.Message);
                throw new Exception("Error deleting file from server", ex);
            }
        }
    }
}
