using FarmShield.Data;
using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string title, string message, NotificationCategory category, NotificationSeverity severity, int? farmId = null, string? targetUrl = null, string? userId = null);
        Task<List<Notification>> GetUnreadNotificationsAsync(int? farmId = null);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int? farmId = null);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string title, string message, NotificationCategory category, NotificationSeverity severity, int? farmId = null, string? targetUrl = null, string? userId = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Category = category,
                Severity = severity,
                FarmId = farmId,
                TargetUrl = targetUrl,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(int? farmId = null)
        {
            var query = _context.Notifications.Where(n => !n.IsRead);
            if (farmId.HasValue)
            {
                query = query.Where(n => n.FarmId == farmId.Value || n.FarmId == null);
            }

            return await query.OrderByDescending(n => n.CreatedAt).Take(10).ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notif = await _context.Notifications.FindAsync(notificationId);
            if (notif != null)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int? farmId = null)
        {
            var query = _context.Notifications.Where(n => !n.IsRead);
            if (farmId.HasValue)
            {
                query = query.Where(n => n.FarmId == farmId.Value || n.FarmId == null);
            }

            var unread = await query.ToListAsync();
            foreach (var item in unread)
            {
                item.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}

