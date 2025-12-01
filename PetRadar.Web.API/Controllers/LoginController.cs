using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetRadar.Core.Domain;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.Services;
using System.Net.Mime;
using System.Security.Claims;

namespace PetRadar.Web.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/gate/[controller]")]
    public class LoginController : PetRadarController
    {
        private readonly IJwtHelper _jwtHelper;
        private readonly IUserDomain _userDomain;

        public LoginController(IJwtHelper jwtHelper, IUserDomain userDomain)
        {
            _jwtHelper = jwtHelper;
            _userDomain = userDomain;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<UserTokenViewModel>> Post([FromBody] LoginModel login, CancellationToken token)
        {
            var userdb = await _userDomain.FindByEmailAndPasswordAsync(login.Username, login.Password, token);
           
            if (userdb == default)
                return Unauthorized();

            if (!userdb.IsActive)
                return Unauthorized();

            var userToken = new UserTokenViewModel();


            userToken = _jwtHelper.GetToken(userdb);

            return Ok(userToken);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<UserTokenViewModel>> PostRefreshAsync([FromBody] RefreshTokenFromUiModel tokenModel, CancellationToken token)
        {

            if (tokenModel == default)
                return BadRequest("Invalid client request");


            var principal = _jwtHelper.GetPrincipalFromRefreshToken(tokenModel.RefreshToken);
            
            if (principal == null)
                return BadRequest("Invalid refresh token");

            var username = principal.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var refreshTokenIsValid = _jwtHelper.ValidateDateFromToken(tokenModel.RefreshToken);
            
            var userDb = await _userDomain.FindByEmailAsync(username, token);

            if (userDb == default || !refreshTokenIsValid)
            {
                return BadRequest("Invalid refresh token");
            }

            var userToken = new UserTokenViewModel();


            userToken = _jwtHelper.GetToken(userDb);

            return Ok(userToken);
        }


    }


}
