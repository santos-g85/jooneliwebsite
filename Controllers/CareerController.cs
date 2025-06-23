using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;
using webjooneli.Services.Interfaces;

namespace webjooneli.Controllers
{
    public class CareerController : Controller
    {
        private readonly ILogger<CareerController> _logger;
        private readonly IFileService _fileService;
        private readonly ICVUploadRepository _careerRepository;
        public CareerController(ILogger<CareerController> logger,IFileService fileService, ICVUploadRepository careerRepository)
        {
            _logger = logger;
            _fileService = fileService;
            _careerRepository = careerRepository;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous] 
       public async Task<IActionResult> CVUploads(CVUploadModel model, IFormFile CVFile)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Received CV upload request for {model.Name}.");

                // Check if CVFile is null or has no length
                if (CVFile == null || CVFile.Length == 0)
                {
                    _logger.LogWarning("CVFile is either null or empty.");
                    ModelState.AddModelError("CVFile", "Please upload a valid CV file.");
                    return View("~/Views/Career/Index.cshtml");
                }

                try
                {
                    _logger.LogInformation("Proceeding with file upload.");

                    var fileid = await _fileService.UploadFileAsync(CVFile, model.Name);
                    _logger.LogInformation($"File uploaded successfully with ID: {fileid}");

                    // Set the fileId to the CVUploadModel
                    model.CVFileId = fileid;
                    model.CreatedAt = DateTime.UtcNow;

                    // Call repository method to upload file and save user info
                    await _careerRepository.CreateCVAsync(model);
                    _logger.LogInformation("Resume data sent successfully.");

                    TempData["SuccessMessage"] = "Your CV has been uploaded successfully!";
                    return RedirectToAction("Index");
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
