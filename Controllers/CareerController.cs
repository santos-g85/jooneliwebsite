using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Models.ViewModels;
using webjooneli.Repository.Interfaces;
using webjooneli.Services.Interfaces;

namespace webjooneli.Controllers
{
    public class CareerController : Controller
    {
        private readonly ILogger<CareerController> _logger;
        private readonly IFileService _fileService;
        private readonly ICVUploadRepository _careerRepository;
        private readonly IJobOpeningRepository _jobOpeningRepository;
        public CareerController(ILogger<CareerController> logger,IFileService fileService, ICVUploadRepository careerRepository, IJobOpeningRepository jobOpeningRepository)
        {
            _logger = logger;
            _fileService = fileService;
            _careerRepository = careerRepository;
            _jobOpeningRepository = jobOpeningRepository;
        }
        public async Task<IActionResult> Index()
        {
            var careerViewModel = new CareerViewModel
            {
                JobOpenings = await _jobOpeningRepository.GetAllJobOpeningsAsync(),
                CVUploadModel = new CVUploadModel()
            };
            return View(careerViewModel);
        }

        public async Task<IActionResult> Details(string id)
        {

            var job = await _jobOpeningRepository.GetJobOpeningByIdAsync(id);
            if (job == null)
            {
                throw new Exception("somethings went wrong!");
            }

            return View(job);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CVUploads(string Name, string ContactNumber, string Email, IFormFile CVFile)
        {
            if (ModelState.IsValid)
            {
                if (CVFile == null || CVFile.Length == 0)
                {
                    ModelState.AddModelError("CVFile", "Please upload a valid CV file.");
                    return View("~/Views/Career/Index.cshtml");
                }

                try
                {
                    // Create the model with user info + timestamp
                    var resume = new CVUploadModel
                    {
                        Name = Name,
                        ContactNumber = ContactNumber,
                        Email = Email,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Call repository method to upload file and save user info
                    await _careerRepository.CreateCVAsync(resume, CVFile);

                    _logger.LogInformation("Resume data sent successfully.");
                    TempData["SuccessMessage"] = "Your CV has been uploaded successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to send resume data: {e.Message}");
                    ModelState.AddModelError("", "An error occurred while sending the resume data. Please try again.");
                    Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return View("~/Views/Career/Index.cshtml");
                }
            }
            return View("~/Views/Career/Index.cshtml");

        }

        }
    }
