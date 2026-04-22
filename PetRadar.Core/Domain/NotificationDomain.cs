using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class NotificationDomain : INotificationDomain
    {
        private readonly INotificationRepository _repo;
        private readonly IPushNotificationService _pushService;

        public NotificationDomain(INotificationRepository repo, IPushNotificationService pushService)
        {
            _repo = repo;
            _pushService = pushService;
        }

        public Task<List<NotificationEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<NotificationEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            return _repo.GetAllByUserIdAsync(userId, token);
        }

        public async Task<NotificationEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var notification = await _repo.FindByIdAsync(id, token);
            if (notification == null)
                return default;

            return notification;
        }

        public async Task<NotificationEntity> CreateAsync(NotificationCreateModel notification, long createdByUserId, CancellationToken token)
        {
            var notificationDb = new NotificationEntity(
                notification.UserId.Value, notification.Type.Value,
                notification.Title, notification.Message,
                notification.Metadata, notification.DeepLink,
                false, null
            );

            notificationDb.CreatedBy = createdByUserId;
            notificationDb.CreatedAt = notificationDb.UpdatedAt = DateTime.UtcNow;
            notificationDb.IsActive = true;

            await _repo.AddAsync(notificationDb);
            await _repo.SaveChangesAsync();

            await _pushService.SendToUserAsync(
                notificationDb.UserId,
                notificationDb.Title,
                notificationDb.Message,
                token);

            return notificationDb;
        }

        public async Task<int> UpdateAsync(NotificationEntity notificationDb, NotificationUpdateModel notification, long modifiedByUserId, CancellationToken token)
        {
            if (notificationDb == default)
                throw new ArgumentNullException(nameof(notificationDb));

            if (notification.Type.HasValue)
                notificationDb.Type = notification.Type.Value;

            if (!string.IsNullOrEmpty(notification.Title))
                notificationDb.Title = notification.Title;

            if (!string.IsNullOrEmpty(notification.Message))
                notificationDb.Message = notification.Message;

            if (notification.Metadata != null)
                notificationDb.Metadata = notification.Metadata;

            if (!string.IsNullOrEmpty(notification.DeepLink))
                notificationDb.DeepLink = notification.DeepLink;

            if (notification.Read.HasValue)
            {
                notificationDb.Read = notification.Read.Value;
                if (notification.Read.Value && !notificationDb.ReadDate.HasValue)
                    notificationDb.ReadDate = DateTimeOffset.UtcNow;
            }

            if (notification.ReadDate.HasValue)
                notificationDb.ReadDate = notification.ReadDate.Value;

            notificationDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(notificationDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteAsync(NotificationEntity notification, long modifiedByUserId, CancellationToken token)
        {
            if (notification == default)
                throw new ArgumentNullException(nameof(notification));

            notification.IsActive = false;

            notification.DeletedByUser(modifiedByUserId);
            _repo.Update(notification);

            return await _repo.SaveChangesAsync();
        }
    }
}
