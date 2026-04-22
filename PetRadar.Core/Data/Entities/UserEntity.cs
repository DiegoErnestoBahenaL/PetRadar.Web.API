using Microsoft.VisualBasic;
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
    public class UserEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(maximumLength: 255), Required]
        public string Email { get; set; } = string.Empty;

        [MaxLength(256), Required]
        public byte[] Password { get; set; } = new byte[256];

        [Required, MaxLength(128)]
        public byte[] Salt { get; set; } = new byte[128];

        [StringLength(maximumLength: 255), Required]
        public string Name { get; set; } = string.Empty;


        [StringLength(maximumLength: 100)]
        public string? LastName { get; set; } = null;

        [StringLength(maximumLength: 20)]
        public string? PhoneNumber { get; set; } = null;

        [StringLength(maximumLength: 255)]
        public string? ProfilePhotoURL { get; set; } = null;

        [Required]
        public RoleEnum Role { get; set; } = RoleEnum.NotSet;

        [StringLength(maximumLength: 255)]
        public string? OrganizationName { get; set; } = null;

        [StringLength(maximumLength: 255)]
        public string? OrganizationAddress { get; set; } = null;

        [StringLength(maximumLength: 20)]
        public string? OrganizationPhone {  get; set; } = null;

        [Required]
        public bool EmailVerified  { get; set; } = false;

        [StringLength(maximumLength: 4096)]
        public string? FcmToken { get; set; } = null;

        public UserEntity() { }

        public UserEntity(string email, byte[] password, byte[] salt, string name, string? lastName, string? phoneNumber, 
            string? organizationName, string? organizationAddress, string? organizationPhone, RoleEnum role, 
            long createdBy)
        {
            Email = email;
            Password = password;
            Salt = salt;
            Name = name;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            OrganizationName = organizationName;
            OrganizationAddress = organizationAddress;
            OrganizationPhone = organizationPhone;
            Role = role;
            CreatedBy = createdBy;
            CreatedAt = UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
