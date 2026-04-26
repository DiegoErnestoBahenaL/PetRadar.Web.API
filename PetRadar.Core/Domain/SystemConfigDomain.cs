using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers.PetRadarProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class SystemConfigDomain  : ISystemConfigDomain
    {

        private readonly ISystemConfigRepository _repository;
        private readonly IPetRadarProcessingHelperService _processingHelperService;

        public SystemConfigDomain(ISystemConfigRepository repository, IPetRadarProcessingHelperService processingHelperService)
        {
            _repository = repository;
            _processingHelperService = processingHelperService;
        }

        public async Task<ConfigsResponse> GetProcessingAPIConfigs()
        {
            return await _processingHelperService.GetConfigs();
        }

        public Task<int> CreateAsync(string key, string value, long createdById, CancellationToken token)
        {
            var config = new SystemConfigEntity
            {
                Key = key,
                Value = value
            };

            config.CreatedByUser(createdById);

            _repository.Add(config);

            return _repository.SaveChangesAsync();
        }


        public Task<int> DeleteAsync(SystemConfigEntity config, long deletedById, CancellationToken token)
        {
            config.IsActive = false;
            config.DeletedByUser(deletedById);
             _repository.Update(config);

            return _repository.SaveChangesAsync();
        }

        public Task<SystemConfigEntity?> FindByIdAsync(long id, CancellationToken token)
        {
           return _repository.FindByIdAsync(id, token);
        }

        public Task<SystemConfigEntity?> FindByKeyAsync(string key, CancellationToken token)
        {
           return _repository.FindByKeyAsync(key, token);
        }

        public Task<List<SystemConfigEntity>> GetAllAsync(CancellationToken token)
        {
            return _repository.GetAllAsync(token);
        }

        public async Task<int> UpdateSystemConfigs(UpdateSystemConfigsModel model, long updatedById, CancellationToken token)
        {

            if (model.YoloConfThreshold.HasValue || model.TopKBreedPredictions.HasValue)
            {
              var result = await _processingHelperService.UpdateConfigs(model.YoloConfThreshold?.ToString(), model.TopKBreedPredictions?.ToString());
            }

            if (model.TopKBreedPredictionThreshold.HasValue)
            {
                var config = await _repository.FindByKeyAsync(Common.Constants.TopBreedPredictionsConfidenceConfigKey, token);
                if (config != null)
                {
                    config.Value = model.TopKBreedPredictionThreshold.Value.ToString();
                    config.UpdatedByUser(updatedById);
                    _repository.Update(config);
                }
            }

            return await _repository.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(SystemConfigEntity config, string? key, string? value, long updatedById, CancellationToken token)
        {

            if (!string.IsNullOrEmpty(key))
                config .Key = key;

            if (!string.IsNullOrEmpty(value))
                config .Value = value;


            config.UpdatedByUser(updatedById);

            _repository.Update(config);


            return await _repository.SaveChangesAsync();
        }
    }
}
