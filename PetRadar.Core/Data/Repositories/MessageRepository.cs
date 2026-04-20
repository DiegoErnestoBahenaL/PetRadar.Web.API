using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class MessageRepository : EntityRepository<MessageEntity>, IMessageRepository
    {
        public MessageRepository(PetRadarDbContext db) : base(db, db.Messages) { }

        public Task<List<MessageEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<MessageEntity>> GetAllBySenderIdAsync(long senderId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.SenderId == senderId);

            return query.ToListAsync(token);
        }

        public Task<List<MessageEntity>> GetAllByRecipientIdAsync(long recipientId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.RecipientId == recipientId);

            return query.ToListAsync(token);
        }

        public Task<List<MessageEntity>> GetAllByMatchIdConversationAsync(long matchId, long recipientId, long senderId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.MatchId == matchId && x.RecipientId == recipientId && x.SenderId == senderId);
            return query.ToListAsync(token);
        }

        public Task<List<MessageEntity>> GetAllByAdoptionAnimalIdConversationAsync(long adoptionAnimalId, long recipientId, long senderId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.AdoptionAnimalId == adoptionAnimalId && x.RecipientId == recipientId && x.SenderId == senderId);
            return query.ToListAsync(token);
        }

        public Task<MessageEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.Messages
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }
    }
}
