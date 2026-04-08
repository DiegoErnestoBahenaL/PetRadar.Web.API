using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetRadar.Core;
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
    public class UserPetsController : PetRadarController
    {
        private readonly ILogger<UserPetsController> _logger;
        private readonly IUserPetDomain _domain;
        private readonly IUserDomain _userDomain;

        public UserPetsController(ILogger<UserPetsController> logger, IUserPetDomain domain, IUserDomain userDomain)
        {
            _logger = logger;
            _domain = domain;
            _userDomain = userDomain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<UserPetViewModel>>> Get(CancellationToken token)
        {
            var pets = await _domain.GetAllAsync(token);

            return Ok(UserPetViewModel.FromList(pets));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<UserPetViewModel>>> GetByUserId([FromRoute] long userId, CancellationToken token)
        {
            var pets = await _domain.GetAllByUserIdAsync(userId, token);

            return Ok(UserPetViewModel.FromList(pets));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<UserPetViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var pet = await _domain.FindByIdAsync(id, token);

            if (pet == default)
                return NotFound();

            return Ok(new UserPetViewModel(pet));
        }

        [HttpGet("{id}/mainpicture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Image.Jpeg, Common.Constants.MediaTypeNamesImagePng)]
        public async Task<IActionResult> GetMainPicture([FromRoute] long id, CancellationToken token)
        {
            var pet = await _domain.FindByIdAsync(id, token);
            if (pet == default)
                return NotFound();

            var path = await _domain.GetMainPicturePath(pet, token);
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] UserPetCreateModel pet, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(pet.UserId.Value, token);

            if (user == default)
                return NotFound();


            var petDb = await _domain.CreateAsync(pet, UserJwt.Id, token);

            return CreatedAtAction(nameof(Get), new { id = petDb.Id }, new UserPetViewModel(petDb));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UserPetUpdateModel pet, CancellationToken token)
        {
            var petdb = await _domain.FindByIdAsync(id, token);

            if (petdb == default)
                return NotFound();

            await _domain.UpdateAsync(petdb, pet, UserJwt.Id, token);
            return NoContent();
        }

        [HttpPut("{id}/mainpicture")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMainPicture([FromRoute] long id, IFormFile file, CancellationToken token)
        {
            var userdb = await _domain.FindByIdAsync(id, token);
            if (userdb == default)
                return NotFound();

            try
            {
                await _domain.UpdateMainPictureAsync(userdb, file, UserJwt.Id, token);
                return NoContent();
            }
            catch (BadHttpRequestException ex)
            {
                // Este BadRequest capturará el error generado cuando la API externa responda con HTTP 400
                // o si la validación de especie en el dominio falla.
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var petdb = await _domain.FindByIdAsync(id, token);

            if (petdb == default)
                return NotFound();

            await _domain.DeleteAsync(petdb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
