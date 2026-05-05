using PetRadar.Common;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class MessageDomain : IMessageDomain
    {
        private readonly IMessageRepository _repo;
        private readonly INotificationDomain _notificationDomain;

        public MessageDomain(IMessageRepository repo, INotificationDomain notificationDomain)
        {
            _repo = repo;
            _notificationDomain = notificationDomain;
        }

        public Task<List<MessageEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<MessageEntity>> GetAllBySenderIdAsync(long senderId, CancellationToken token)
        {
            return _repo.GetAllBySenderIdAsync(senderId, token);
        }

        public Task<List<MessageEntity>> GetAllByRecipientIdAsync(long recipientId, CancellationToken token)
        {
            return _repo.GetAllByRecipientIdAsync(recipientId, token);
        }

        public Task<List<MessageEntity>> GetAllByMatchIdConversationAsync(long matchId, long recipientId, long senderId, CancellationToken token)
        {
            return _repo.GetAllByMatchIdConversationAsync(matchId, recipientId, senderId, token);
        }
        
        public Task<int> CountUnreadMessagesByMatchIdAsync(long matchId, long recipientId, long senderId, CancellationToken token)
        {
            return _repo.CountUnreadMessagesByMatchIdAsync(matchId, recipientId, senderId, token);
        }

        public Task<List<MessageEntity>> GetAllByAdoptionAnimalIdConversationAsync(long adoptionAnimalId, long recipientId, long senderId, CancellationToken token)
        {
            return _repo.GetAllByAdoptionAnimalIdConversationAsync(adoptionAnimalId, recipientId, senderId, token);
        }

        public Task<int> CountUnreadMessagesByAdoptionAnimalIdAsync(long adoptionAnimalId, long recipientId, long senderId, CancellationToken token)
        {
            return _repo.CountUnreadMessagesByAdoptionAnimalIdAsync(adoptionAnimalId, recipientId, senderId, token);
        }

        public async Task<MessageEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var message = await _repo.FindByIdAsync(id, token);
            if (message == null)
                return default;

            return message;
        }

        public async Task<MessageEntity> CreateAsync(MessageCreateModel message, long createdByUserId, CancellationToken token)
        {
            var messageDb = new MessageEntity(
                message.SenderId, message.RecipientId, message.Content,
                message.MatchId, message.AdoptionAnimalId
            );

            messageDb.CreatedBy = createdByUserId;
            messageDb.CreatedAt = messageDb.UpdatedAt = DateTime.UtcNow;
            messageDb.IsActive = true;

            await _repo.AddAsync(messageDb);
            await _repo.SaveChangesAsync();

            string matchMessage = "Has recibido un nuevo mensaje sobre la mascota reportada";
            string adoptionMessage = "Has recibido un nuevo mensaje sobre la mascota en adopcion";

            string messageBody = string.Empty;

            if (messageDb.MatchId != null)
            {
                messageBody = matchMessage;
            }
            else if (adoptionMessage != null)
            {
                messageBody = adoptionMessage;
            }

            // Create a notification for the user about the new message
            await _notificationDomain.CreateAsync(
                 new NotificationCreateModel(
                     message.RecipientId,
                     NotificationTypeEnum.Message,
                     "Tienes un nuevo mensaje!",
                     messageBody,
                     null,
                     null),
                 Constants.SuperAdminId,
            token);

            return messageDb;
        }

        public async Task<int> UpdateAsync(MessageEntity messageDb, MessageUpdateModel message, long modifiedByUserId, CancellationToken token)
        {
            if (messageDb == default)
                throw new ArgumentNullException(nameof(messageDb));

            if (!string.IsNullOrEmpty(message.Content))
                messageDb.Content = message.Content;

            if (message.Read.HasValue)
                messageDb.Read = message.Read.Value;

            if (message.ReadDate.HasValue)
                messageDb.ReadDate = message.ReadDate.Value;

            messageDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(messageDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteAsync(MessageEntity message, long modifiedByUserId, CancellationToken token)
        {
            if (message == default)
                throw new ArgumentNullException(nameof(message));

            message.IsActive = false;

            message.DeletedByUser(modifiedByUserId);
            _repo.Update(message);

            return await _repo.SaveChangesAsync();
        }
    }
}
