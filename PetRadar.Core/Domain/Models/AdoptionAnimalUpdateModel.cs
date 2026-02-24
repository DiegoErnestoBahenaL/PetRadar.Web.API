using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain.Models
{
    public class AdoptionAnimalUpdateModel
    {
        public AdoptionAnimalUpdateModel() { }

        [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "The field Name must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSpeciesEnum? Species { get; set; }

        [StringLength(maximumLength: 100, ErrorMessage = "The field Breed must be a string with a maximum length of {1}")]
        public string? Breed { get; set; }

        [StringLength(maximumLength: 100,  ErrorMessage = "The field Color must be a string with a maximum length of {1}")]
        public string? Color { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSexEnum? Sex { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSizeEnum? Size { get; set; }

        public decimal? ApproximateAge { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(maximumLength: 500, ErrorMessage = "The field Description must be a string with a maximum length of {1}")]
        public string? Description { get; set; }

        public bool? IsNeutered { get; set; }

        [StringLength(maximumLength: 100, ErrorMessage = "The field Personality must be a string with a maximum length of {1}")]
        public string? Personality { get; set; }

        public bool? GoodWithKids { get; set; }

        public bool? GoodWithDogs { get; set; }

        public bool? GoodWithCats { get; set; }

        public bool? IsVaccinated { get; set; }

        public bool? NeedsSpecialCare { get; set; }

        [StringLength(maximumLength: 500,  ErrorMessage = "The field SpecialCareDetails must be a string with a maximum length of {1}")]
        public string? SpecialCareDetails { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoptionStatusEnum? Status { get; set; }

        public DateTimeOffset? AdoptionDate { get; set; }

        public long? AdopterId { get; set; }

        public AdoptionAnimalUpdateModel(
            string? name, PetSpeciesEnum? species, string? breed, string? color,
            PetSexEnum? sex, PetSizeEnum? size, decimal? approximateAge,
            decimal? weight, string? description, bool? isNeutered,
            string? personality, bool? goodWithKids, bool? goodWithDogs,
            bool? goodWithCats, bool? isVaccinated, bool? needsSpecialCare, string? specialCareDetails,
            AdoptionStatusEnum? status, DateTimeOffset? adoptionDate, long? adopterId)
        {
            Name = name;
            Species = species;
            Breed = breed;
            Color = color;
            Sex = sex;
            Size = size;
            ApproximateAge = approximateAge;
            Weight = weight;
            Description = description;
            IsNeutered = isNeutered;
            Personality = personality;
            GoodWithKids = goodWithKids;
            GoodWithDogs = goodWithDogs;
            GoodWithCats = goodWithCats;
            IsVaccinated = isVaccinated;
            NeedsSpecialCare = needsSpecialCare;
            SpecialCareDetails = specialCareDetails;
            Status = status;
            AdoptionDate = adoptionDate;
            AdopterId = adopterId;
        }
    }
}
