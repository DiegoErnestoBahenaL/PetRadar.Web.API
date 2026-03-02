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
    public class UserPetEntity : PetEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required, StringLength(maximumLength: 100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity ParentUser { get; set; }

        public DateTimeOffset? BirthDate { get; set; }
      
        [StringLength(maximumLength: 255)]
        public string? Allergies { get; set; }

        [StringLength(maximumLength: 500)]
        public string? MedicalNotes { get; set; }

        public UserPetEntity() : base() {}

        public UserPetEntity
        (
            long userId, string name, PetSpeciesEnum species, string? breed, string? color,
            PetSexEnum? sex, PetSizeEnum? size, DateTimeOffset? birthDate, decimal? approximateAge,
            decimal? weight, string? description, bool? isNeutered, string? allergies, string? medicalNotes
        )
        : base(species, breed, color, sex, size, approximateAge, weight, description, photoURL: null, additionalPhotosURL: null, isNeutered)
        {
            Name = name;
            UserId = userId;
            BirthDate = birthDate;
            Allergies = allergies;
            MedicalNotes = medicalNotes;
        }
       
    }
}
