using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers.PetRadarProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface ISystemConfigDomain
    {
        Task<List<SystemConfigEntity>> GetAllAsync(CancellationToken token);
        Task<SystemConfigEntity?> FindByKeyAsync(string key, CancellationToken token);
        Task<SystemConfigEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<ConfigsResponse> GetProcessingAPIConfigs();
        Task<int> UpdateSystemConfigs(UpdateSystemConfigsModel model, long updatedById, CancellationToken token);
        Task<int> CreateAsync (string key, string value, long createdById, CancellationToken token);
        Task<int> UpdateAsync (SystemConfigEntity config, string? key, string? value, long updatedById, CancellationToken token);
        Task<int> DeleteAsync (SystemConfigEntity config, long deletedById, CancellationToken token);
    }
}
