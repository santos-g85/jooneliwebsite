using Microsoft.AspNetCore.Mvc;
using webjooneli.Repository.Interfaces;
using webjooneli.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace webjooneli.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessagesRepository _messagesRepository;

        public MessageController(IMessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        [Authorize(Roles = "Admin")]
        // GET: MessageController
        public async Task<IActionResult> Index()
        {
            try
            {
                var messages = await _messagesRepository.GetAllMessagesSortedByDateAsync();
                return View(messages);
            }
            catch (Exception ex)
            {
                // Log the error and show a user-friendly message
                TempData["Error"] = "An error occurred while retrieving messages.";
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
                    return RedirectToAction(nameof(Index));
                }

                return View(message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while retrieving the message.";
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: MessageController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MessageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessagesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.UtcNow;
                    await _messagesRepository.CreateMessageAsync(model);
                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction(nameof(Create));
                }
                catch (Exception ex)
                {
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
                    return RedirectToAction(nameof(Index));
                }

                return View(message);
            }
            catch (Exception ex)
            {
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
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the message.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
