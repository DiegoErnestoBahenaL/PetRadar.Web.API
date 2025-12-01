using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace PetRadar.Web.API.Services
{
    public class RefreshTokenFromUiModel
    {
        [StringLength(3000, MinimumLength = 10)]
        public string RefreshToken { get; set; }


        public RefreshTokenFromUiModel()
        {

        }

        public RefreshTokenFromUiModel(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
