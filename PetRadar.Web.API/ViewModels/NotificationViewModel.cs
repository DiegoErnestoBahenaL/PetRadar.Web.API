using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class NotificationViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public string? DeepLink { get; set; }
        public bool Read { get; set; }
        public DateTimeOffset? ReadDate { get; set; }

        public NotificationViewModel() { }

        public NotificationViewModel(NotificationEntity entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            Type = entity.Type.ToString();
            Title = entity.Title;
            Message = entity.Message;
            Metadata = entity.Metadata;
            DeepLink = entity.DeepLink;
            Read = entity.Read;
            ReadDate = entity.ReadDate;
        }

        public static List<NotificationViewModel> FromList(List<NotificationEntity> entities)
        {
            var viewModels = new List<NotificationViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new NotificationViewModel(entity));
            }
            return viewModels;
        }
    }
}
