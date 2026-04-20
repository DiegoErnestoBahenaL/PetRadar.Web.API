using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IReportRepository : IEntityRepository<ReportEntity>
    {
        Task<List<ReportEntity>> GetAllAsync(CancellationToken token);
        Task<List<ReportEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<ReportEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<List<ReportEntity>> GetAllByLostReportTypeAsync(PetSpeciesEnum species, CancellationToken token);
        Task<List<ReportEntity>> GetAllByStrayReportTypeAsync(PetSpeciesEnum species, CancellationToken token);
    }
}
