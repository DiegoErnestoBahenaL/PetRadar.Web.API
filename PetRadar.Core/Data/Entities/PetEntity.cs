using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    public abstract class PetEntity : Entity, IEntity
    {
        [Required, StringLength(maximumLength: 100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public PetSpeciesEnum Species { get; set; } = PetSpeciesEnum.NotSet;

        [StringLength(maximumLength: 100)]
        public string? Breed { get; set; }

        [StringLength(maximumLength: 100)]
        public string? Color { get; set; }

        public PetSexEnum? Sex { get; set; }

        public PetSizeEnum? Size { get; set; }

        public decimal? ApproximateAge { get; set; }
        public decimal? Weight { get; set; }

        [StringLength(maximumLength: 500)]
        public string? Description { get; set; }

        [StringLength(maximumLength: 255)]
        public string? PhotoURL { get; set; }

        [StringLength(maximumLength: 255)]
        public string? AdditionalPhotosURL { get; set; }

        public bool? IsNeutered { get; set; }

        protected PetEntity() { }

        protected PetEntity(
            string name, PetSpeciesEnum species, string? breed, string? color, PetSexEnum? sex, 
            PetSizeEnum? size, decimal? approximateAge, decimal? weight, string? description, 
            string? photoURL, string? additionalPhotosURL, bool? isNeutered
        )
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
            PhotoURL = photoURL;
            AdditionalPhotosURL = additionalPhotosURL;
            IsNeutered = isNeutered;
        }
    }
}
