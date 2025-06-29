using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using webjooneli.Services.Interfaces;

public class ImageService : IImageService
{
    private readonly ILogger<ImageService> _logger;
    private const string UploadsFolder = "images/uploads";
    private const int MaxFileSizeMB = 5;
    private readonly string[] _allowedMimeTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public ImageService(ILogger<ImageService> logger)
    {
        _logger = logger;
    }

    public async Task<string> UploadImageAsync(IFormFile imageFile)
    {
        try
        {
            // Validate file
            ValidateImageFile(imageFile);

            // Create uploads directory if it doesn't exist
            var uploadsPath = GetUploadsPath();
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}.webp";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Convert and save image
            await ConvertAndSaveImage(imageFile, filePath);

            // Return relative path
            return Path.Combine(UploadsFolder, fileName).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            throw;
        }
    }

    public bool DeleteImage(string imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image");
            return false;
        }
    }

    private void ValidateImageFile(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new ArgumentException("No file uploaded");

        if (imageFile.Length > MaxFileSizeMB * 1024 * 1024)
            throw new ArgumentException($"File size exceeds {MaxFileSizeMB}MB limit");

        if (!_allowedMimeTypes.Contains(imageFile.ContentType.ToLower()))
            throw new ArgumentException("Invalid file type. Only image files are allowed");
    }

    private string GetUploadsPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", UploadsFolder);
    }

    private async Task ConvertAndSaveImage(IFormFile imageFile, string outputPath)
    {
        using var inputStream = imageFile.OpenReadStream();
        using var image = await Image.LoadAsync(inputStream);

        // Optional: Resize if needed
        if (image.Width > 1920 || image.Height > 1080)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(1920, 1080),
                Mode = ResizeMode.Max
            }));
        }

        var encoder = new WebpEncoder
        {
            Quality = 80,
            Method = WebpEncodingMethod.BestQuality,
            TransparentColorMode = WebpTransparentColorMode.Preserve
        };

        await image.SaveAsync(outputPath, encoder);
    }
}