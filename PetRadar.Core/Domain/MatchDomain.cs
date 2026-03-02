using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class MatchDomain : IMatchDomain
    {
        private readonly IMatchRepository _repo;

        public MatchDomain(IMatchRepository repo)
        {
            _repo = repo;
        }

        public Task<List<MatchEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<MatchEntity>> GetAllByLostReportIdAsync(long lostReportId, CancellationToken token)
        {
            return _repo.GetAllByLostReportIdAsync(lostReportId, token);
        }

        public Task<List<MatchEntity>> GetAllByStrayReportIdAsync(long strayReportId, CancellationToken token)
        {
            return _repo.GetAllByStrayReportIdAsync(strayReportId, token);
        }

        public async Task<MatchEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var match = await _repo.FindByIdAsync(id, token);
            if (match == null)
                return default;

            return match;
        }

        public async Task<MatchEntity> CreateAsync(MatchCreateModel match, long createdByUserId, CancellationToken token)
        {
            var matchDb = new MatchEntity(
                match.LostReportId, match.StrayReportId, match.DistanceInKM,
                match.Notes, match.ConfirmationDate
            );

            matchDb.CreatedBy = createdByUserId;
            matchDb.CreatedAt = matchDb.UpdatedAt = DateTime.UtcNow;
            matchDb.IsActive = true;

            await _repo.AddAsync(matchDb);
            await _repo.SaveChangesAsync();
            return matchDb;
        }

        public async Task<int> UpdateAsync(MatchEntity matchDb, MatchUpdateModel match, long modifiedByUserId, CancellationToken token)
        {
            if (matchDb == default)
                throw new ArgumentNullException(nameof(matchDb));

            if (match.Score.HasValue)
                matchDb.Score = match.Score.Value;

            if (match.DistanceInKM.HasValue)
                matchDb.DistanceInKM = match.DistanceInKM.Value;

            if (match.Status.HasValue)
                matchDb.Status = match.Status.Value;

            if (!string.IsNullOrEmpty(match.Notes))
                matchDb.Notes = match.Notes;

            if (match.ConfirmationDate.HasValue)
                matchDb.ConfirmationDate = match.ConfirmationDate.Value;

            matchDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(matchDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteAsync(MatchEntity match, long modifiedByUserId, CancellationToken token)
        {
            if (match == default)
                throw new ArgumentNullException(nameof(match));

            match.IsActive = false;

            match.DeletedByUser(modifiedByUserId);
            _repo.Update(match);

            return await _repo.SaveChangesAsync();
        }
    }
}
