using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    public class AdoptionRequest
    {
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string HouseType { get; set; } = string.Empty;
        public bool HasGarden { get; set; } = false;
        public string LivesWith { get; set; } = string.Empty;
        public bool HasOtherPets { get; set; } = false;
        public string PreviousExperience { get; set; } = string.Empty;
        public string Motivations { get; set; } = string.Empty;
        public string AdditionalComments { get; set; } = string.Empty;
    }
}
