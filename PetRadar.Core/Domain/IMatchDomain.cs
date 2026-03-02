using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IMatchDomain
    {
        Task<List<MatchEntity>> GetAllAsync(CancellationToken token);
        Task<List<MatchEntity>> GetAllByLostReportIdAsync(long lostReportId, CancellationToken token);
        Task<List<MatchEntity>> GetAllByStrayReportIdAsync(long strayReportId, CancellationToken token);
        Task<MatchEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<MatchEntity> CreateAsync(MatchCreateModel match, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(MatchEntity matchDb, MatchUpdateModel match, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(MatchEntity match, long modifiedByUserId, CancellationToken token);
    }
}
