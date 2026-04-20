using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
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

        public Task<List<ReportEntity>> GetAllByLostReportTypeAsync(PetSpeciesEnum species, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.ReportType == ReportTypeEnum.Lost && x.Species == species);
            return query.ToListAsync(token);
        }   

        public Task<List<ReportEntity>> GetAllByStrayReportTypeAsync(PetSpeciesEnum species, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.ReportType == ReportTypeEnum.Stray && x.Species == species);
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
