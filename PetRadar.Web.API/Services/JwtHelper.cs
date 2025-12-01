using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using NuGet.Configuration;
using PetRadar.Core.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetRadar.Web.API.Services
{
    public class JwtHelper : IJwtHelper
    {
        readonly string _issuer;
        readonly string _audience;
        readonly TimeSpan _expiration;
        readonly double _expirationHoursRefreshToken;
        private readonly string _signingKey;


        readonly SymmetricSecurityKey _key;
        readonly SigningCredentials _credentials;
        readonly JwtSecurityTokenHandler _tokenHandler = new();

        public JwtHelper()
        {
            _issuer = "petradar.com";
            _audience = "petradar.com";
            _expiration = TimeSpan.FromHours(24);
            _expirationHoursRefreshToken = 48;

            _signingKey = "RedactedRedactedRedactedRedacted";

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RedactedRedactedRedactedRedacted"));
            _credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        }

        public JwtHelper(string signingKey, TimeSpan expiration, double refreshTokenExpirationHours, string issuer, string audience)
        {
            _issuer = issuer;
            _audience = audience;
            _expiration = expiration;
            _expirationHoursRefreshToken = refreshTokenExpirationHours;

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            _credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        }

        public JwtSecurityToken Generate(UserEntity userdb, bool isRefreshToken)
        {
            var claims = new List<Claim>();

            long organizationId = 0;
            bool subGoalsAccess = false;

            if (isRefreshToken)
            {
                claims.AddRange(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userdb.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userdb.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, userdb.Name),
                new Claim(JwtRegisteredClaimNames.FamilyName, userdb.LastName),

            });
                var jwt = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(_expirationHoursRefreshToken),
                _credentials);
                return jwt;
            }
            else
            {
                claims.AddRange(new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, userdb.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userdb.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, userdb.Name),
            new Claim(JwtRegisteredClaimNames.FamilyName, userdb.LastName),

            // TODO: hardcoded role
            new Claim("Role", userdb.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)

            // I'm not gonna blacklist/whitelist tokens so... disable is for now
            //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            });


                var jwt = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(_expiration),
                _credentials);
                return jwt;
            }

        }

        public UserTokenViewModel GetToken(UserEntity userdb)
        {
            var token = Generate(userdb, false);
            var strToken = _tokenHandler.WriteToken(token);

            var refreshToken = Generate(userdb,true);
            var strRefreshToken = _tokenHandler.WriteToken(refreshToken);
            var refreshTokenModelExpirationTime = DateTime.UtcNow.AddHours(_expirationHoursRefreshToken);

            return new UserTokenViewModel(strToken, token.ValidTo, strRefreshToken, refreshTokenModelExpirationTime);
        }

        public ClaimsPrincipal GetPrincipalFromRefreshToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public bool ValidateDateFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            if (securityToken.ValidTo <= DateTime.UtcNow)
                return false;
            return true;
        }
    }
}
