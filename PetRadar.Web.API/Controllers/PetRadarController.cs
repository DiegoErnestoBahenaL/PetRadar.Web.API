using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using PetRadar.Core.Data.Entities;
using System.Security.Claims;

namespace PetRadar.Web.API.Controllers
{
    public abstract class PetRadarController : ControllerBase
    {
        UserJwt _userJwt;

        /// <summary>
        /// "Logged in" user
        /// </summary>
        protected UserJwt UserJwt { get { return GetUserJwt(); } }

        public PetRadarController() { }

        protected UserJwt GetUserJwt()
        {
            if (_userJwt == default)
            {
                var claims = HttpContext.User.Claims;
                var sub = GetClaim(ClaimTypes.NameIdentifier, claims);
                var email = GetClaim(ClaimTypes.Email, claims);
                var givenName = GetClaim(ClaimTypes.GivenName, claims);
                var lastName = GetClaim(ClaimTypes.Surname, claims);
                var role = GetClaim(ClaimTypes.Role, claims);
                long id = long.Parse(sub.Value);

                if (!Enum.TryParse(role.Value, out RoleEnum roleEnum))
                    roleEnum = RoleEnum.User;

                _userJwt = new UserJwt(id, email.Value, givenName.Value, lastName.Value, roleEnum);
            }
            return _userJwt;
        }


        static Claim GetClaim(string type, IEnumerable<Claim> claims)
        {
            var claim = claims.FirstOrDefault(x => x.Type == type);
            if (claim == default)
                throw new InvalidOperationException("Jwt does not have: " + type);

            if (string.IsNullOrEmpty(claim.Value))
                throw new InvalidOperationException("Jwt claim value is null or empty: " + type);

            return claim;
        }
    }
}
