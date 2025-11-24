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

        [StringLength(maximumLength: 255), Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(maximumLength: 255), Required]
        public string Name { get; set; } = string.Empty;


        [StringLength(maximumLength: 100)]
        public string? LastName { get; set; }

        [StringLength(maximumLength: 20)]
        public string? PhoneNumber { get; set; }

        [StringLength(maximumLength: 255)]
        public string? ProfilePhotoURL { get; set; }

        [Required]
        public RoleEnum Role { get; set; }

        [StringLength(maximumLength: 255)]
        public string? OrganizationName { get; set; }

        [StringLength(maximumLength: 255)]
        public string? OrganizationAddress { get; set; }

        [StringLength(maximumLength: 20)]
        public string? OrganizationPhone {  get; set; }

        [Required]
        public bool EmailVerified  { get; set; }

        public UserEntity() { }

        public UserEntity(string email, string password, string name, string lastName, string phoneNumber, RoleEnum role)
        {
            Email = email;
            Password = password;
            Name = name;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Role = role;

            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }
    }
}
