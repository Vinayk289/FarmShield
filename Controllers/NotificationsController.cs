using FarmShield.Data;
using FarmShield.Models.Enums;
using FarmShield.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public NotificationsController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(NotificationCategory? categoryFilter, bool? unreadOnly)
        {
            var query = _context.Notifications.Include(n => n.Farm).AsQueryable();

            if (categoryFilter.HasValue)
            {
                query = query.Where(n => n.Category == categoryFilter.Value);
            }

            if (unreadOnly.HasValue && unreadOnly.Value)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.UnreadOnly = unreadOnly;
            ViewBag.TotalUnread = notifications.Count(n => !n.IsRead);

            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id, string? returnUrl = null)
        {
            await _notificationService.MarkAsReadAsync(id);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync();
            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }
    }
}

