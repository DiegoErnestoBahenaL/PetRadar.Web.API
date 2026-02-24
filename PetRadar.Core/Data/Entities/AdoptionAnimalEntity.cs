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
    public class AdoptionAnimalEntity : PetEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long ShelterId { get; set; }

        [ForeignKey(nameof(ShelterId))]
        public UserEntity ShelterUser { get; set; }

        [StringLength(maximumLength: 100)]
        public string? Personality { get; set; }
        public bool? GoodWithKids { get; set; }
        public bool? GoodWithDogs { get; set; }
        public bool? GoodWithCats { get; set; }
        public bool? IsVaccinated { get; set; }
        
        public bool? NeedsSpecialCare { get; set; }

        [StringLength(maximumLength: 500)]
        public string? SpecialCareDetails { get; set; }
        public AdoptionStatusEnum Status { get; set; } = AdoptionStatusEnum.Available;
        public DateTimeOffset? AdoptionDate { get; set; }

        public long? AdopterId { get; set; }

        [ForeignKey(nameof(AdopterId))]
        public UserEntity? AdopterUser { get; set; }

        public int Views { get; set; } = 0;

        public AdoptionAnimalEntity() : base () { }

        public AdoptionAnimalEntity(
            long shelterId, string name, PetSpeciesEnum species, string? breed, string? color,
            PetSexEnum? sex, PetSizeEnum? size, decimal? approximateAge,
            decimal? weight, string? description, bool? isNeutered, 
            string? personality, bool? goodWithKids, bool? goodWithDogs, 
            bool? goodWithCats, bool? isVaccinated, bool? needsSpecialCare, string? specialCareDetails
           
        )
        : base(name, species, breed, color, sex, size, approximateAge, weight, description, photoURL: null, additionalPhotosURL: null, isNeutered)
        {
            ShelterId = shelterId;
            Personality = personality;
            GoodWithKids = goodWithKids;
            GoodWithDogs = goodWithDogs;
            GoodWithCats = goodWithCats;
            IsVaccinated = isVaccinated;
            NeedsSpecialCare = needsSpecialCare;
            SpecialCareDetails = specialCareDetails;
        }
    }
}
