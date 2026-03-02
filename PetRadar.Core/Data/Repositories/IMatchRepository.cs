using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IMatchRepository : IEntityRepository<MatchEntity>
    {
        Task<List<MatchEntity>> GetAllAsync(CancellationToken token);
        Task<List<MatchEntity>> GetAllByLostReportIdAsync(long lostReportId, CancellationToken token);
        Task<List<MatchEntity>> GetAllByStrayReportIdAsync(long strayReportId, CancellationToken token);
        Task<MatchEntity?> FindByIdAsync(long id, CancellationToken token);
    }
}
