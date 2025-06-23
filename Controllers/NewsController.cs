using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;

namespace webjooneli.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsRepository newsRepository, ILogger<NewsController> logger)
        {
            _newsRepository = newsRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        // GET: /News
        public async Task<IActionResult> Index()
        {
            var newsList = await _newsRepository.GetAllNewsAsync();
            return View(newsList);
        }

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
        [Authorize(Roles = "Admin")]
        // GET: /News/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: /News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsModel news, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(news);

            try
            {
                news.CreatedAt = DateTime.UtcNow;
                await _newsRepository.CreateNewsAsync(news, imageFile);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news");
                ModelState.AddModelError("", "An error occurred while saving the news.");
                return View(news);
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: /News/Edit/5
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
        // POST: /News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NewsModel news)
        {
            if (id != news.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(news);

            await _newsRepository.UpdateNewsAsync(id, news);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
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
            await _newsRepository.DeleteNewsAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
