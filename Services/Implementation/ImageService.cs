using MongoDB.Bson;
using webjooneli.Settings;
using webjooneli.Services.Interfaces;

namespace webjooneli.Services.Implementation
{
    public class ImageService : IImageService
    {
        private readonly MongoDbContext _mongoDbContext;

        public ImageService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        // Upload image to GridFS
        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No image file selected for upload");

            using (var stream = imageFile.OpenReadStream())
            {
                // Upload the image to GridFS and get the file ID
                var fileId = await _mongoDbContext.GridFsBucket.UploadFromStreamAsync(imageFile.FileName, stream);
                return fileId.ToString(); // Return the file ID as a string
            }
        }

        // Download image from GridFS
        public async Task<byte[]> DownloadImageAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("Invalid file ID");

            var objectId = new ObjectId(fileId); // Convert the string fileId to ObjectId

            // Download the image's data from GridFS
            using (var stream = await _mongoDbContext.GridFsBucket.OpenDownloadStreamAsync(objectId))
            {
                var imageBytes = new byte[stream.Length];
                await stream.ReadAsync(imageBytes, 0, (int)stream.Length);
                return imageBytes;
            }
        }
    }
}
