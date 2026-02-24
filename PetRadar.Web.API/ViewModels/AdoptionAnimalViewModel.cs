using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class AdoptionAnimalViewModel
    {
        public long Id { get; set; }
        public long ShelterId { get; set; }
        public string Name { get; set; } = string.Empty;
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
        public string? Personality { get; set; }
        public bool? GoodWithKids { get; set; }
        public bool? GoodWithDogs { get; set; }
        public bool? GoodWithCats { get; set; }
        public bool? IsVaccinated { get; set; }
        public bool? NeedsSpecialCare { get; set; }
        public string? SpecialCareDetails { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset? AdoptionDate { get; set; }
        public long? AdopterId { get; set; }
        public int Views { get; set; }

        public AdoptionAnimalViewModel() { }

        public AdoptionAnimalViewModel(AdoptionAnimalEntity entity)
        {
            Id = entity.Id;
            ShelterId = entity.ShelterId;
            Name = entity.Name;
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
            Personality = entity.Personality;
            GoodWithKids = entity.GoodWithKids;
            GoodWithDogs = entity.GoodWithDogs;
            GoodWithCats = entity.GoodWithCats;
            IsVaccinated = entity.IsVaccinated;
            NeedsSpecialCare = entity.NeedsSpecialCare;
            SpecialCareDetails = entity.SpecialCareDetails;
            Status = entity.Status.ToString();
            AdoptionDate = entity.AdoptionDate;
            AdopterId = entity.AdopterId;
            Views = entity.Views;
        }

        public static List<AdoptionAnimalViewModel> FromList(List<AdoptionAnimalEntity> entities)
        {
            var viewModels = new List<AdoptionAnimalViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new AdoptionAnimalViewModel(entity));
            }
            return viewModels;
        }
    }
}
