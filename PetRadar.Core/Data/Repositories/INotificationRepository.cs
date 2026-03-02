using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface INotificationRepository : IEntityRepository<NotificationEntity>
    {
        Task<List<NotificationEntity>> GetAllAsync(CancellationToken token);
        Task<List<NotificationEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<NotificationEntity?> FindByIdAsync(long id, CancellationToken token);
    }
}
