using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;

namespace webjooneli.Controllers
{
    public class JobApplicationController : Controller
    {
        private readonly IJobApplicationRepository _applicationRepository;
        private readonly IJobOpeningRepository _jobOpeningRepository;

        public JobApplicationController(IJobApplicationRepository applicationRepository, IJobOpeningRepository jobOpeningRepository)
        {
            _applicationRepository = applicationRepository;
            _jobOpeningRepository = jobOpeningRepository;
        }

      
        // GET: JobApplication/Create
        public IActionResult Create()
        {

            var jobOpenings = _jobOpeningRepository.GetAllJobOpeningsAsync(); 
            //var model = new JobApplicationModel
            //{
            //     FullName= jobOpenings.FullName,
                 
            //};

            return View(jobOpenings);
        }

        // POST: JobApplication/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobApplicationModel application)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create the job application
                    await _applicationRepository.CreateApplicationAsync(application);
                    TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            // If validation failed, or on error, return to Create view with existing job openings
            var jobOpenings = await _jobOpeningRepository.GetAllJobOpeningsAsync();
            ViewBag.JobOpenings = jobOpenings;
            return View(application);
        }

       
        // GET: JobApplication/Index
        public async Task<IActionResult> Index()
        {
            // Retrieve all job applications
            var applications = await _applicationRepository.GetAllApplicationsAsync();
            return View(applications);
        }

        // GET: JobApplication/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the application by Id
            var application = await _applicationRepository.GetApplicationByIdAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            // Fetch the associated job opening (if needed)
            //application.JobOpening = await _applicationRepository.GetJobOpeningByApplicationIdAsync(id);
            return View(application);
        }

 
        // GET: JobApplication/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the application by Id
            var application = await _applicationRepository.GetApplicationByIdAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: JobApplication/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Delete the application by Id
            await _applicationRepository.DeleteApplicationAsync(id);
            TempData["SuccessMessage"] = "The application has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

       
    }
}
