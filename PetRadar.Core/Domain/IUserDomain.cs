using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IUserDomain
    {
        Task<List<UserEntity>> GetAllAsync(CancellationToken token);
        Task<UserEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<UserEntity> CreateAsync(UserCreateModel user, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(UserEntity userdb, UserUpdateModel user, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(UserEntity user, long modifiedByUserId, CancellationToken token);
    }
}
