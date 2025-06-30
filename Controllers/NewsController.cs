using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using webjooneli.Services.Implementation;
using webjooneli.Services.Interfaces;

namespace webjooneli.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ILogger<NewsController> _logger;
        private readonly IImageService _imageService;
        public NewsController(INewsRepository newsRepository, 
            ILogger<NewsController> logger,
            IImageService imageService)
        {
            _newsRepository = newsRepository;
            _logger = logger;
            _imageService = imageService;
        }

        // GET: /News
        public async Task<IActionResult> Index()
        {
            var news = await _newsRepository.GetNewsByDateAsync();
            if (news == null)
                return NotFound();

            return View(news);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            List<NewsModel> newsList = await _newsRepository.GetAllNewsAsync();
            return View(newsList);
        }



        [Unauthenticated404]
        // GET: /News/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var news = await _newsRepository.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound();

            return View(news);
        }



        public async Task<IActionResult> NewsDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var news = await _newsRepository.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound();

            return View(news);
        }


        [Unauthenticated404]
        // GET: /News/Create
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        //[Authorize(Roles = "admin")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsModel news,
            IFormFile? imageFile)
        {
            _logger.LogInformation($"image received in controller {imageFile}");
            if (!ModelState.IsValid)
            {
                return View(news);
            }

            try
            {
                // Handle image upload if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    _logger.LogInformation("trying to get imageurl");
                    news.ImageId = await _imageService.UploadImageAsync(imageFile);
                    _logger.LogInformation("got the iamge url");
                }

                news.CreatedAt = DateTime.UtcNow;

                await _newsRepository.CreateNewsAsync(news);

                TempData["SuccessMessage"] = "News created successfully!";
                return RedirectToAction(nameof(AdminIndex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news");
                ModelState.AddModelError("", "Error creating news: " + ex.Message);
                return View(news);
            }
        }




        [Unauthenticated404]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var news = await _newsRepository.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound();

            return View(news);
        }

        [Authorize(Roles = "Admin")]
        public bool DeleteImage(string imagePath)
        {
            _logger.LogInformation("Deleting image with path {FilePath}", imagePath);
            try
            {
                _imageService.DeleteImage(imagePath);
                _logger.LogInformation("imageP file deleted successfully with path {FilePath}", imagePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can't delete CV file with path {FilePath}. Error: {ErrorMessage}",
                    imagePath, ex.Message);
                return false;
            }
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NewsModel news, IFormFile? newImageFile)
        {
            try
            {
                if (id != news.Id)
                {
                    _logger.LogWarning("ID mismatch in Edit request");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in Edit request");
                    return View(news);
                }

                var existingNews = await _newsRepository.GetNewsByIdAsync(id);
                if (existingNews == null)
                {
                    _logger.LogWarning("News not found for ID: {Id}", id);
                    TempData["ErrorMessage"] = "News not found";
                    return RedirectToAction(nameof(AdminIndex));
                }

                // Handle image update if new file was provided
                if (newImageFile != null && newImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingNews.ImageId))
                    {
                        try
                        {
                            DeleteImage(existingNews.ImageId);
                            _logger.LogInformation("Deleted old image: {ImageId}", existingNews.ImageId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to delete old image");
                            // Continue with new image upload anyway
                        }
                    }

                    // Upload new image
                    var imageUploadResult = await _imageService.UploadImageAsync(newImageFile);
                   
                    news.ImageId = imageUploadResult;
                    _logger.LogInformation("Uploaded new image: {ImageId}", news.ImageId);
                }
                else
                {
                    // Keep existing image if no new file was provided
                    news.ImageId = existingNews.ImageId;
                }

                await _newsRepository.UpdateNewsAsync(id, news);
                TempData["SuccessMessage"] = "News updated successfully";
                return RedirectToAction(nameof(AdminIndex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news");
                TempData["ErrorMessage"] = "An error occurred while updating the news";
                return View(news);
            }
        }



        [Unauthenticated404]
        // GET: /News/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var news = await _newsRepository.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound();

            return View(news);
        }
       

        [Authorize(Roles = "Admin")]
        // POST: /News/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var news = await _newsRepository.GetNewsByIdAsync(id);
            var imagepath = news.ImageId;
            await _newsRepository.DeleteNewsAsync(id);
            DeleteImage(imagepath);
            return RedirectToAction(nameof(AdminIndex));
        }


        [AllowAnonymous]
        public async Task<IActionResult> Subscribe(string email)
        {
            _logger.LogInformation("User trying to subscribe to newsletter with email: {Email}", email);
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Index));
            }
            var usersubinfo = new NewsSubscriptionModel
            {
                Email = email,
                Subscribed = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Trying to create subscription!");
                 await _newsRepository.CreateSubscription(usersubinfo);
                TempData["SuccessMessage"] = "Successfully subscribed to newsletter!";
            }
            catch(Exception ex)
            {
                _logger.LogError($"Subscription creation failed, {ex.Message}");
                TempData["ErrorMessage"] = "Something went wrong";
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
