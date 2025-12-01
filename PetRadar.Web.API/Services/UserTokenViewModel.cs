namespace PetRadar.Web.API.Services
{
    public class UserTokenViewModel
    {
        public string Token { get; set; }
        public DateTimeOffset TokenValidTo { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpiryTime { get; set; }
        public UserTokenViewModel()
        {

        }

        public UserTokenViewModel
        (
            string token,
            DateTimeOffset validTo,
            string refreshToken,
            DateTimeOffset refreshTokenExpiryTime
        )
        {
            Token = token;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
            TokenValidTo = validTo;
        }
    }
}
