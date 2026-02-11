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
    public class UserPetUpdateModel
    {
        public UserPetUpdateModel() { }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "The field Name must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSpeciesEnum? Species { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "The field Breed must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Breed { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "The field Color must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Color { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSexEnum? Sex { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSizeEnum? Size { get; set; }

        public DateTimeOffset? BirthDate { get; set; }

        public decimal? ApproximateAge { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Description must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Description { get; set; }

        public bool? IsNeutered { get; set; }

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field Allergies must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Allergies { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field MedicalNotes must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? MedicalNotes { get; set; }

        public UserPetUpdateModel(
            string? name, PetSpeciesEnum? species, string? breed, string? color,
            PetSexEnum? sex, PetSizeEnum? size, DateTimeOffset? birthDate, decimal? approximateAge,
            decimal? weight, string? description, bool? isNeutered, string? allergies, string? medicalNotes)
        {
            Name = name;
            Species = species;
            Breed = breed;
            Color = color;
            Sex = sex;
            Size = size;
            BirthDate = birthDate;
            ApproximateAge = approximateAge;
            Weight = weight;
            Description = description;
            IsNeutered = isNeutered;
            Allergies = allergies;
            MedicalNotes = medicalNotes;
        }
    }
}
