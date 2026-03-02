using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class ReportRepository : EntityRepository<ReportEntity>, IReportRepository
    {
        public ReportRepository(PetRadarDbContext db) : base(db, db.Reports) { }

        public Task<List<ReportEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<ReportEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.UserId == userId);

            return query.ToListAsync(token);
        }

        public Task<ReportEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.Reports
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }
    }
}
