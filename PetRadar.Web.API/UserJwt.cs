using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API
{
    public class UserJwt
    {
        public long Id { get; protected set; }
        public string Email { get; protected set; }
        public string Name { get; protected set; }
        public string LastName { get; protected set; }
        public RoleEnum Role { get; protected set; }


        public UserJwt(long userId, string email, string name, string lastName, RoleEnum role)
        {
            Id = userId;
            Email = email;
            Name = name;
            LastName = lastName;
            Role = role;
        }
    }
}