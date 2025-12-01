using System.ComponentModel.DataAnnotations;

namespace PetRadar.Web.API.Services
{
    public class LoginModel
    {
        [Required, StringLength(255)]
        public string Username { get; set; }
        [Required, StringLength(255)]
        public string Password { get; set; }

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
