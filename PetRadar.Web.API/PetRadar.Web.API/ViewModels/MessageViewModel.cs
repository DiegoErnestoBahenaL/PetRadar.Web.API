using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class MessageViewModel
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public long? ReportId { get; set; }
        public long? MatchId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool Read { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public DateTimeOffset? ReadDate { get; set; }

        public MessageViewModel() { }

        public MessageViewModel(MessageEntity entity)
        {
            Id = entity.Id;
            SenderId = entity.SenderId;
            RecipientId = entity.RecipientId;
            ReportId = entity.ReportId;
            MatchId = entity.MatchId;
            Content = entity.Content;
            Read = entity.Read;
            SentAt = entity.SentAt;
            ReadDate = entity.ReadDate;
        }

        public static List<MessageViewModel> FromList(List<MessageEntity> entities)
        {
            var viewModels = new List<MessageViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new MessageViewModel(entity));
            }
            return viewModels;
        }
    }
}
