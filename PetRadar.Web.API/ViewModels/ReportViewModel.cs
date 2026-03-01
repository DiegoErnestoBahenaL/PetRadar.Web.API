using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class ReportViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? UserPetId { get; set; }
        public string Species { get; set; } = string.Empty;
        public string? Breed { get; set; }
        public string? Color { get; set; }
        public string? Sex { get; set; }
        public string? Size { get; set; }
        public decimal? ApproximateAge { get; set; }
        public decimal? Weight { get; set; }
        public string? Description { get; set; }
        public string? PhotoURL { get; set; }
        public string? AdditionalPhotosURL { get; set; }
        public bool? IsNeutered { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string ReportStatus { get; set; } = string.Empty;
        public bool? HasCollar { get; set; }
        public bool? HasTag { get; set; }
        public DateTimeOffset ReportDate { get; set; }
        public DateTimeOffset? IncidentDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AddressText { get; set; }
        public int SearchRadiusMeters { get; set; }
        public bool UseAlternateContact { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public bool OffersReward { get; set; }
        public decimal? RewardAmount { get; set; }
        public int Views { get; set; }

        public ReportViewModel() { }

        public ReportViewModel(ReportEntity entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            UserPetId = entity.UserPetId;
            Species = entity.Species.ToString();
            Breed = entity.Breed;
            Color = entity.Color;
            Sex = entity.Sex?.ToString();
            Size = entity.Size?.ToString();
            ApproximateAge = entity.ApproximateAge;
            Weight = entity.Weight;
            Description = entity.Description;
            PhotoURL = entity.PhotoURL;
            AdditionalPhotosURL = entity.AdditionalPhotosURL;
            IsNeutered = entity.IsNeutered;
            ReportType = entity.ReportType.ToString();
            ReportStatus = entity.ReportStatus.ToString();
            HasCollar = entity.HasCollar;
            HasTag = entity.HasTag;
            ReportDate = entity.ReportDate;
            IncidentDate = entity.IncidentDate;
            Latitude = entity.Location?.Y;
            Longitude = entity.Location?.X;
            AddressText = entity.AddressText;
            SearchRadiusMeters = entity.SearchRadiusMeters;
            UseAlternateContact = entity.UseAlternateContact;
            ContactName = entity.ContactName;
            ContactPhone = entity.ContactPhone;
            ContactEmail = entity.ContactEmail;
            OffersReward = entity.OffersReward;
            RewardAmount = entity.RewardAmount;
            Views = entity.Views;
        }

        public static List<ReportViewModel> FromList(List<ReportEntity> entities)
        {
            var viewModels = new List<ReportViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new ReportViewModel(entity));
            }
            return viewModels;
        }
    }
}
