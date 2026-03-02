using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IMessageRepository : IEntityRepository<MessageEntity>
    {
        Task<List<MessageEntity>> GetAllAsync(CancellationToken token);
        Task<List<MessageEntity>> GetAllBySenderIdAsync(long senderId, CancellationToken token);
        Task<List<MessageEntity>> GetAllByRecipientIdAsync(long recipientId, CancellationToken token);
        Task<MessageEntity?> FindByIdAsync(long id, CancellationToken token);
    }
}
