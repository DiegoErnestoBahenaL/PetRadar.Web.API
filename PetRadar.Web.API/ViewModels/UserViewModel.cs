using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class UserViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePhotoURL { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? OrganizationName { get; set; }
        public string? OrganizationAddress { get; set; }
        public string? OrganizationPhone { get; set; }

        public UserViewModel() { }

        public UserViewModel(UserEntity entity)
        {
            Id = entity.Id;
            Email = entity.Email;
            Name = entity.Name;
            LastName = entity.LastName;
            PhoneNumber = entity.PhoneNumber;
            ProfilePhotoURL = entity.ProfilePhotoURL;
            Role = entity.Role.ToString();
            OrganizationName = entity.OrganizationName;
            OrganizationAddress = entity.OrganizationAddress;
            OrganizationPhone = entity.OrganizationPhone;
        }

        public static List<UserViewModel> FromList (List<UserEntity> entities)
        {
            var userViewModels = new List<UserViewModel>();
            foreach (var entity in entities)
            {
                userViewModels.Add(new UserViewModel(entity));
            }
            return userViewModels;
        }
    }
}
