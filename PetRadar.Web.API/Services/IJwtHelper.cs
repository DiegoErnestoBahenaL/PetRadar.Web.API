using PetRadar.Core.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PetRadar.Web.API.Services
{
    public interface IJwtHelper
    {
        JwtSecurityToken Generate(UserEntity userdb, bool isRefreshToken);
        UserTokenViewModel GetToken(UserEntity userdb);
        ClaimsPrincipal GetPrincipalFromRefreshToken(string token);
        bool ValidateDateFromToken(string token);
    }
}
