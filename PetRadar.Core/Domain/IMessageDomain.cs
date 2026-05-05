using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IMessageDomain
    {
        Task<List<MessageEntity>> GetAllAsync(CancellationToken token);
        Task<List<MessageEntity>> GetAllBySenderIdAsync(long senderId, CancellationToken token);
        Task<List<MessageEntity>> GetAllByRecipientIdAsync(long recipientId, CancellationToken token);
        Task<List<MessageEntity>> GetAllByMatchIdConversationAsync(long matchId, long recipientId, long senderId, CancellationToken token);
        Task<List<MessageEntity>> GetAllByAdoptionAnimalIdConversationAsync(long adoptionAnimalId, long recipientId, long senderId, CancellationToken token);
        Task<MessageEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<MessageEntity> CreateAsync(MessageCreateModel message, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(MessageEntity messageDb, MessageUpdateModel message, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(MessageEntity message, long modifiedByUserId, CancellationToken token);
        Task<int> CountUnreadMessagesByMatchIdAsync(long matchId, long recipientId, long senderId, CancellationToken token);
        Task<int> CountUnreadMessagesByAdoptionAnimalIdAsync(long adoptionAnimalId, long recipientId, long senderId, CancellationToken token);
    }
}
