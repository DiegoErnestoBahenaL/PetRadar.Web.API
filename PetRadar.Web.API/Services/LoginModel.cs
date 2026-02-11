using System.ComponentModel.DataAnnotations;

namespace PetRadar.Web.API.Services
{
    public class LoginModel
    {
        [Required, StringLength(255)]
        public string Username { get; set; } = string.Empty;
        [Required, StringLength(255)]
        public string Password { get; set; } = string.Empty;

        public LoginModel()
        {

        }

        public LoginModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
