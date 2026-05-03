using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetRadar.Core.Domain;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.Services;
using System.Net;
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
                return BadRequest(Constants.BadRequestProblemDetails("Invalid client request"));

            var principal = _jwtHelper.GetPrincipalFromRefreshToken(tokenModel.RefreshToken);

            if (principal == null)
                return BadRequest(Constants.BadRequestProblemDetails("Invalid refresh token"));

            var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
                return BadRequest(Constants.BadRequestProblemDetails("Invalid refresh token"));

            var username = emailClaim.Value;

            var refreshTokenIsValid = _jwtHelper.ValidateDateFromToken(tokenModel.RefreshToken);

            var userDb = await _userDomain.FindByEmailAsync(username, token);

            if (userDb == default || !refreshTokenIsValid)
            {
                return BadRequest(Constants.BadRequestProblemDetails("Invalid refresh token"));
            }

            var userToken = new UserTokenViewModel();

            userToken = _jwtHelper.GetToken(userDb);

            return Ok(userToken);
        }

        [HttpGet("VerifyEmail/{token}")]

        public async Task<IActionResult> VerifyEmail([FromRoute] string token, CancellationToken cancellationToken)
        {
            bool validToken = _jwtHelper.ValidateDateFromToken(token);

            if (validToken)
            {
                var claimsPrincipal = _jwtHelper.GetPrincipalFromRefreshToken(token);

                long userId = long.Parse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");

                var userDb = await _userDomain.FindByIdAsync(userId, cancellationToken);

                if (userDb == default)
                {
                    return BadRequest();
                }

                if (userDb.EmailVerified)
                {
                    return BadRequest(Constants.BadRequestProblemDetails("Email is already verified"));
                }

                await _userDomain.VerifyEmailAsync(userDb, userId);

                return Ok("Email verified successfully");
            }
            else
            {
                return BadRequest(Constants.BadRequestProblemDetails("Invalid token"));
            }
        }

        [HttpPost("recoverpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]

        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordModel recoverPasswordModel, CancellationToken cancellationToken)
        {
            var userDb = await _userDomain.FindByEmailAsync(recoverPasswordModel.Email, cancellationToken);

            if (userDb == default)
            {
                return BadRequest(Constants.BadRequestProblemDetails(HttpStatusCode.BadRequest.ToString()));
            }

            var resultSuccessful = await _userDomain.RecoverPasswordAsync(userDb, UserJwt.Id, cancellationToken);

            if (!resultSuccessful)
            {
                var details = Constants.InternalServerErrorProblemDetails("Error sending recovery email");


                return Problem(title: details.Title, detail: details.Detail, statusCode: details.Status);
            }

            return Ok();
        }
    }
}
