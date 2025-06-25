using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using webjooneli.Services.Interfaces;

namespace webjooneli.Services.Implementation
{
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageService(ILogger<ImageService> logger, IHttpContextAccessor httpContextAccessor)
        {
            
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadImageAsync(IFormFile upload)
        {
            _logger.LogInformation("received image to convert it to webp and save to wwwroot!");
            try
            {
                // Check if the file is null or empty  
                if (upload == null || upload.Length == 0)
                    throw new ArgumentException("No file uploaded.");

                // Validate file type (allow only image types)  
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp" };
                if (!allowedTypes.Contains(upload.ContentType.ToLower()))
                    throw new ArgumentException("Invalid file type. Only image files are allowed.");

                _logger.LogInformation("creating directory in wwwroot if it doesnot exits!");
                // Create uploads folder if it doesn't exist  
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                _logger.LogInformation($"directory on name {uploadsFolder} created!");

                // Generate a unique filename for the image (using GUID)  
                var fileName = Guid.NewGuid().ToString() + ".webp";
                var filePath = Path.Combine(uploadsFolder, fileName);

                _logger.LogInformation("attempting to save the image to the folder");
                // Convert the uploaded image to WebP format and save it  
                using (var inputStream = upload.OpenReadStream())
                using (var image = await Image.LoadAsync(inputStream))
                {
                    var encoder = new WebpEncoder()
                    {
                        Quality = 80, // Adjust quality (0-100)  
                        Method = WebpEncodingMethod.BestQuality,
                        TransparentColorMode = WebpTransparentColorMode.Preserve
                    };
                    await image.SaveAsync(filePath, encoder);
                }
                _logger.LogInformation("image saved in folder!");
                // Use IHttpContextAccessor to get the current HTTP context and construct the file URL  
                var request = _httpContextAccessor.HttpContext.Request;
                var fileUrl = $"{request.Scheme}://{request.Host}/uploads/{fileName}";
                _logger.LogInformation("returning the imagepath");
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error uploading image: " + ex.Message);
                throw new Exception("Image upload failed", ex);
            }
        }

        //public async Task<byte[]> DownloadImageAsync(string relativePath)
        //{
        //    var filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));

        //    if (!File.Exists(filePath))
        //        throw new FileNotFoundException("Image not found");

        //    return await File.ReadAllBytesAsync(filePath);
        //}
    }
}
