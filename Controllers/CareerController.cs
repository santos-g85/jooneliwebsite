using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public CareerController(ILogger<CareerController> logger,
            IFileService fileService,
            ICVUploadRepository careerRepository, 
            IJobOpeningRepository jobOpeningRepository)
        {
            _logger = logger;
            _fileService = fileService;
            _careerRepository = careerRepository;
            _jobOpeningRepository = jobOpeningRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CareerViewModel
            {
                JobOpenings = await _jobOpeningRepository.GetAllJobOpeningsAsync() ?? new List<JobOpeningModel>(),
                CVUploadModel = new CVUploadModel()
            };

            return View(model);
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


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CVUploads()
        {
            try
            {
                var cvList = await _careerRepository.GetAllCVsAsync();
                return View(cvList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving CVs: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while retrieving CVs. Please try again later.");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return View(new List<CVUploadModel>());
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [RequestSizeLimit(20*1024*1024)]
        public async Task<IActionResult> CVUploads(string Name, string Email, string ContactNumber, IFormFile CVFile)
        {
           _logger.LogInformation($"CV info received with file:{CVFile}!");

            if (string.IsNullOrWhiteSpace(Name) || 
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrEmpty(ContactNumber))
            {
                _logger.LogWarning("Invalid Cv form submitted!");
            }
            else
            {
                try
                {
                    var fileId = await _fileService.UploadFileAsync(CVFile);
                    var filePath = fileId.FilePath;
                    _logger.LogInformation($"Received file id from fileservice with file id {filePath}");

                    var resume = new CVUploadModel
                    {
                        Name = Name,
                        ContactNumber = ContactNumber,
                        Email = Email,
                        CVFileId = filePath.ToString(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _logger.LogInformation("new cvupload model {Name},{email}", resume.Name,resume.Email);
                    await _careerRepository.CreateCVAsync(resume);
                    _logger.LogInformation("cv sent successfully!");
                    TempData["SuccessMessage"] = "Your CV has been sent to Jooneli Inc!";
                }
                catch(Exception ex)
                {
                    _logger.LogError($"{ex.Message}");
                }
            }
           return View("~/Views/Career/Index.cshtml");

        }

        [HttpGet]
        public async Task<IActionResult> DownloadCVFile([FromQuery] string filePath)
        {
            var result = await _fileService.DownloadFileAsync(filePath);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return File(
                result.FileContent,
                result.ContentType,
                result.FileName
            );
        }

        public bool DeleteCvFile(string cvFilePath)
        {
            _logger.LogInformation("Deleting CV file with path {FilePath}", cvFilePath);
            try
            {
                _fileService.DeleteFile(cvFilePath);
                _logger.LogInformation("CV file deleted successfully with path {FilePath}", cvFilePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can't delete CV file with path {FilePath}. Error: {ErrorMessage}",
                    cvFilePath, ex.Message);
                return false;
            }
        }

        
        public async Task<IActionResult> DeleteCV(string Id)
        {
            _logger.LogWarning("Attempting to delete CV!");
            try
            {
                var cv = await _careerRepository.GetCVByIdAsync(Id);
                if (cv == null)
                {
                    _logger.LogError("CV not found with ID {Id}", Id);
                    TempData["ErrorMessage"] = "CV not found!";
                    return RedirectToAction(nameof(CVUploads));
                }

                _logger.LogInformation("Deleting CV file with path {CvFilePath}", cv.CVFileId);

                if (!DeleteCvFile(cv.CVFileId))
                {
                    _logger.LogError("Failed to delete CV file at path {CvFilePath}", cv.CVFileId);
                    TempData["ErrorMessage"] = "Failed to delete CV file!";
                    return RedirectToAction(nameof(CVUploads));
                }

                await _careerRepository.DeleteCVAsync(Id);
                _logger.LogWarning("CV deleted successfully!");

                TempData["SuccessMessage"] = "CV deleted successfully!";
                return RedirectToAction(nameof(CVUploads));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CV: {ErrorMessage}", ex.Message);
                TempData["ErrorMessage"] = "An error occurred while deleting the CV!";
                return RedirectToAction(nameof(CVUploads));
            }
        }
    }
}
