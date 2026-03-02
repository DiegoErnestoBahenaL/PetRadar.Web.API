using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class NotificationRepository : EntityRepository<NotificationEntity>, INotificationRepository
    {
        public NotificationRepository(PetRadarDbContext db) : base(db, db.Notifications) { }

        public Task<List<NotificationEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<NotificationEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.UserId == userId);

            return query.ToListAsync(token);
        }

        public Task<NotificationEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.Notifications
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }
    }
}
