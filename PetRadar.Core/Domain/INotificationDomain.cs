using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface INotificationDomain
    {
        Task<List<NotificationEntity>> GetAllAsync(CancellationToken token);
        Task<List<NotificationEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<NotificationEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<NotificationEntity> CreateAsync(NotificationCreateModel notification, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(NotificationEntity notificationDb, NotificationUpdateModel notification, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(NotificationEntity notification, long modifiedByUserId, CancellationToken token);
    }
}
