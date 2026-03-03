using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using PetRadar.Common;
using PetRadar.Core;
using PetRadar.Core.Data;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.ViewModels;
using System.Net.Mime;

namespace PetRadar.Web.API.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(RoleEnum.SuperAdmin) + "," + nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.User) + "," + nameof(RoleEnum.Organization))]
    [Route("/api/[controller]")]
    public class UsersController : PetRadarController
    {

        private readonly ILogger<UsersController> _logger;
        private readonly IUserDomain _domain;

        public UsersController(ILogger<UsersController> logger, IUserDomain domain)
        {
            _logger = logger;
            _domain = domain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<UserViewModel>>> Get(CancellationToken token)
        {

            var users = await _domain.GetAllAsync(token);

            return Ok(UserViewModel.FromList(users));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<UserViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var user = await _domain.FindByIdAsync(id, token);

            if (user == default)
                return NotFound();

            return Ok(new UserViewModel(user));
        }

        [HttpGet("{id}/profilepicture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Image.Jpeg, Common.Constants.MediaTypeNamesImagePng)]
        public async Task<IActionResult> GetProfilePicture([FromRoute] long id, CancellationToken token)
        {
            var user = await _domain.FindByIdAsync(id, token);
            if (user == default)
                return NotFound();

            var path = await _domain.GetUserProfilePicturePath(user, token);
            if (path == null)
                return NotFound();

            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                string mimeType = Common.Constants.GetMimeType(path);

                return File(bytes, mimeType);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error while trying to retrieve image");
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] UserCreateModel user, CancellationToken token)
        {
            UserEntity userdb;
            try 
            {
                // If UserJwt is null, it means the user is creating their own account,
                // so we can set createdBy to 0 or some default value.


                userdb = await _domain.CreateAsync(user, UserJwt.Id, token);

                return CreatedAtAction(nameof(Get), new { id = userdb.Id }, new UserViewModel(userdb));

            }
            //If UserJwt.Id throws InvalidOperationException, it means the user is creating their own account,
            // so we can set createdByUserID to 1 or some default value.
            catch (InvalidOperationException)
            {
                userdb = await _domain.CreateAsync(user, createdByUserId: 1, token);

                return CreatedAtAction(nameof(Get), new { id = userdb.Id }, new UserViewModel(userdb));
            }
            catch (PetRadarException ex)
            {
                if (ex.Message.Contains("Can't create duplicated data"))
                {
                    return Conflict(ex.Message);
                }     

                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}/profilepicture")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfilePicture([FromRoute] long id, IFormFile file, CancellationToken token)
        {
            var userdb = await _domain.FindByIdAsync(id, token);
            if (userdb == default)
                return NotFound();

            await _domain.UpdateProfilePictureAsync(userdb, file, UserJwt.Id, token);
            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UserUpdateModel user, CancellationToken token)
        {
            var userdb = await _domain.FindByIdAsync(id, token);

            if (userdb == default)
                return NotFound();
            //Use JWT info
            await _domain.UpdateAsync(userdb, user, UserJwt.Id, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var userdb = await _domain.FindByIdAsync(id, token);

            if (userdb == default)
                return NotFound();
            //Use JWT info
            await _domain.DeleteAsync(userdb, UserJwt.Id, token);
            return NoContent();
        }

    }
}
