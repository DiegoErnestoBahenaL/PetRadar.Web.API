using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain.Models
{
    public class UserUpdateModel
    {
        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field Email is not valid, check the format"), EmailAddress]
        public string? Email { get; set; } 

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field Password must be in range of {2} and {1}")]
        public string? Password { get; set; } 

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field Name must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Name { get; set; } 

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field LastName must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? LastName { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = "The field PhoneNumber must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? PhoneNumber { get; set; } 

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field OrganizationName must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? OrganizationName { get; set; }

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field OrganizationAddress must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? OrganizationAddress { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = "The field OrganizationPhone must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? OrganizationPhone { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public RoleEnum? Role { get; set; }


        public UserUpdateModel() { }

        public UserUpdateModel(string? email, string? password, string? name, string? lastName, string? phoneNumber, 
            string? organizationName, string? organizationAddress, string? organizationPhone, RoleEnum? role)
        {
            Email = email;
            Password = password;
            Name = name;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            OrganizationName = organizationName;
            OrganizationAddress = organizationAddress;
            OrganizationPhone = organizationPhone;
            Role = role;
        }
    }
}
