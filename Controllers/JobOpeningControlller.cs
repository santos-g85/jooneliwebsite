using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;

namespace webjooneli.Controllers
{
    public class JobOpeningController : Controller
    {
        private readonly IJobOpeningRepository _jobOpeningRepository;
        private readonly ILogger<JobOpeningController> _logger;
        public JobOpeningController(IJobOpeningRepository jobOpeningRepository, 
            ILogger<JobOpeningController> logger)
        {
            _jobOpeningRepository = jobOpeningRepository;
            _logger = logger;
        }
       
        [Authorize(Roles ="Admin")]
        // Get all job openings
        public async Task<IActionResult> Index()
        {
            var jobOpenings = await _jobOpeningRepository.GetAllJobOpeningsAsync();
            return View(jobOpenings);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobOpeningModel jobOpening)
        {
            if (ModelState.IsValid)
            {
                await _jobOpeningRepository.CreateJobOpeningAsync(jobOpening);
                return RedirectToAction(nameof(Index));
            }
            return View(jobOpening);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var jobOpening = await _jobOpeningRepository.GetJobOpeningByIdAsync(id);
            if (jobOpening == null)
            {
                return NotFound();
            }
            return View(jobOpening);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, 
            JobOpeningModel jobOpening)
        {
            if (id != jobOpening.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _jobOpeningRepository.UpdateJobOpeningAsync(id, jobOpening);
                return RedirectToAction(nameof(Index));
            }
            return View(jobOpening);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            await _jobOpeningRepository.DeleteJobOpeningAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
