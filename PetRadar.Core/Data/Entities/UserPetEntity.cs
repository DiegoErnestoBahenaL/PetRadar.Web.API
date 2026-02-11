using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    public class UserPetEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity ParentUser { get; set; }

        [Required, StringLength(maximumLength: 100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public PetSpeciesEnum Species { get; set; }
        
        [StringLength(maximumLength: 100)]
        public string? Breed { get; set; }

        [StringLength(maximumLength: 100)]
        public string? Color { get; set; }

        public PetSexEnum? Sex { get; set; }

        public PetSizeEnum? Size { get; set; }

        public DateTimeOffset? BirthDate { get; set; }
        
        public decimal? ApproximateAge { get; set; }
        public decimal?  Weight { get; set; }

        [StringLength(maximumLength: 500)]
        public string? Description { get; set; }

        [StringLength(maximumLength: 255)]
        public string? PhotoURL { get; set; }

        [StringLength(maximumLength: 255)]
        public string? AdditionalPhotosURL { get; set; }

        public bool? IsNeutered { get; set; }

        [StringLength(maximumLength: 255)]
        public string? Allergies { get; set; }

        [StringLength(maximumLength: 500)]
        public string? MedicalNotes { get; set; }

        public UserPetEntity() 
        {
            ParentUser = new UserEntity();
        }

        public UserPetEntity
        (
            long userId, string name, PetSpeciesEnum species, string? breed, string? color, 
            PetSexEnum? sex, PetSizeEnum? size, DateTimeOffset? birthDate, decimal? approximateAge, 
            decimal? weight, string? description, bool? isNeutered, string? allergies, string? medicalNotes
        )
        {
            UserId = userId;
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
