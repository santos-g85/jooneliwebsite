using Microsoft.AspNetCore.Mvc;
using webjooneli.Repository.Interfaces;
using webjooneli.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace webjooneli.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessagesRepository _messagesRepository;
        private ILogger<MessageController> _logger;
        public MessageController(IMessagesRepository messagesRepository,
            ILogger<MessageController> logger)
        {
            _messagesRepository = messagesRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        // GET: MessageController
        public async Task<IActionResult> AdminIndex()
        {
            try
            {
                var messages = await _messagesRepository.GetAllMessagesSortedByDateAsync();
                _logger.LogInformation("messages sorted by date retrieved");
                return View(messages);
            }
            catch (Exception ex)
            {
                // Log the error and show a user-friendly message
                TempData["Error"] = "An error occurred while retrieving messages.";
                _logger.LogError($"{ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: MessageController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var message = await _messagesRepository.GetMessagesByIdAsync(id);
                if (message == null)
                {
                    TempData["Error"] = "Message not found.";
                    _logger.LogWarning("message not found");
                    return RedirectToAction(nameof(Index));
                }

                return View(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                TempData["Error"] = "An error occurred while retrieving the message.";
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: MessageController/Create
        public IActionResult Index()
        {
            return View();
        }

        // POST: MessageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MessagesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.UtcNow;
                    await _messagesRepository.CreateMessageAsync(model);
                    _logger.LogInformation("message created successfully");
                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex.Message}");
                    TempData["Error"] = $"An error occurred while creating the message {ex.Message}.";
                    return View(model);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: MessageController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var message = await _messagesRepository.GetMessagesByIdAsync(id);
                if (message == null)
                {
                    TempData["Error"] = "Message not found.";
                    _logger.LogWarning("Message not found.");
                    return RedirectToAction(nameof(Index));
                }

                return View(message);
            }
            catch (Exception ex)
            { 
                _logger.LogError($"{ex.Message}");
                TempData["Error"] = "An error occurred while retrieving the message for deletion.";
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: MessageController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _messagesRepository.DeleteMessagesAsync(id);
                _logger.LogInformation("Message delted");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                TempData["Error"] = "An error occurred while deleting the message.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
