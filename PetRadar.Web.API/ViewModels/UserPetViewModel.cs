using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class UserPetViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string? Breed { get; set; }
        public string? Color { get; set; }
        public string? Sex { get; set; }
        public string? Size { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public decimal? ApproximateAge { get; set; }
        public decimal? Weight { get; set; }
        public string? Description { get; set; }
        public string? PhotoURL { get; set; }
        public string? AdditionalPhotosURL { get; set; }
        public bool? IsNeutered { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalNotes { get; set; }

        public UserPetViewModel() { }

        public UserPetViewModel(UserPetEntity entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            Name = entity.Name;
            Species = entity.Species.ToString();
            Breed = entity.Breed;
            Color = entity.Color;
            Sex = entity.Sex?.ToString();
            Size = entity.Size?.ToString();
            BirthDate = entity.BirthDate;
            ApproximateAge = entity.ApproximateAge;
            Weight = entity.Weight;
            Description = entity.Description;
            PhotoURL = entity.PhotoURL;
            AdditionalPhotosURL = entity.AdditionalPhotosURL;
            IsNeutered = entity.IsNeutered;
            Allergies = entity.Allergies;
            MedicalNotes = entity.MedicalNotes;
        }

        public static List<UserPetViewModel> FromList(List<UserPetEntity> entities)
        {
            var viewModels = new List<UserPetViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new UserPetViewModel(entity));
            }
            return viewModels;
        }
    }
}
