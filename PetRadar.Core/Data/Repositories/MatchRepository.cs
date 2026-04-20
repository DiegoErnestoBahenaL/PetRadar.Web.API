using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class MatchRepository : EntityRepository<MatchEntity>, IMatchRepository
    {
        public MatchRepository(PetRadarDbContext db) : base(db, db.Matches) { }

        public Task<List<MatchEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Include(x => x.LostReport)
                .Include(x => x.StrayReport)
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<MatchEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Include(x => x.LostReport)
                .Include(x => x.StrayReport)
                .Where(x => x.IsActive == true && (x.LostReport.UserId == userId || x.StrayReport.UserId == userId));
            return query.ToListAsync(token);
        }

        public Task<List<MatchEntity>> GetAllByLostReportIdAsync(long lostReportId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Include(x => x.LostReport)
                .Include(x => x.StrayReport)
                .Where(x => x.IsActive == true && x.LostReportId == lostReportId);

            return query.ToListAsync(token);
        }

        public Task<List<MatchEntity>> GetAllByStrayReportIdAsync(long strayReportId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Include(x => x.LostReport)
                .Include(x => x.StrayReport)
                .Where(x => x.IsActive == true && x.StrayReportId == strayReportId);

            return query.ToListAsync(token);
        }

        public Task<MatchEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.Matches
                .Include(x => x.LostReport)
                .Include(x => x.StrayReport)
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }

        public Task<MatchEntity?> FindByReportsIdsAsync(long lostReportId, long strayReportId, CancellationToken token)
        {
            return _dbContext.Matches
                .Include(x => x.LostReport)
                .Include(x => x.StrayReport)
                .Where(x => x.IsActive == true && x.LostReportId == lostReportId && x.StrayReportId == strayReportId)
                .FirstOrDefaultAsync(token);
        }
    }
}
