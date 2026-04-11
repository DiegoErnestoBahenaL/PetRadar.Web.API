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
    public class AdoptionAnimalsController : PetRadarController
    {
        private readonly ILogger<AdoptionAnimalsController> _logger;
        private readonly IAdoptionAnimalDomain _domain;
        private readonly IUserDomain _userDomain;

        public AdoptionAnimalsController(ILogger<AdoptionAnimalsController> logger, IAdoptionAnimalDomain domain, IUserDomain userDomain)
        {
            _logger = logger;
            _domain = domain;
            _userDomain = userDomain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<AdoptionAnimalViewModel>>> Get(CancellationToken token)
        {
            var animals = await _domain.GetAllAsync(token);

            return Ok(AdoptionAnimalViewModel.FromList(animals));
        }

        [HttpGet("shelter/{shelterId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<AdoptionAnimalViewModel>>> GetByShelterId([FromRoute] long shelterId, CancellationToken token)
        {
            var animals = await _domain.GetAllByShelterIdAsync(shelterId, token);

            return Ok(AdoptionAnimalViewModel.FromList(animals));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<AdoptionAnimalViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var animal = await _domain.FindByIdAsync(id, token);

            if (animal == default)
                return NotFound(Constants.NotFoundProblemDetails);

            return Ok(new AdoptionAnimalViewModel(animal));
        }

        [HttpGet("{id}/mainpicture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Image.Jpeg, Common.Constants.MediaTypeNamesImagePng)]
        public async Task<IActionResult> GetMainPicture([FromRoute] long id, CancellationToken token)
        {
            var animal = await _domain.FindByIdAsync(id, token);
            if (animal == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var path = await _domain.GetMainPicturePath(animal, token);
            if (path == null)
                return NotFound(Constants.NotFoundProblemDetails);
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
            return NotFound(Constants.NotFoundProblemDetails);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] AdoptionAnimalCreateModel animal, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(animal.ShelterId.Value, token);

            if (user == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var animalDb = await _domain.CreateAsync(animal, UserJwt.Id, token);

            return CreatedAtAction(nameof(Get), new { id = animalDb.Id }, new AdoptionAnimalViewModel(animalDb));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] AdoptionAnimalUpdateModel animal, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);

            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            if (animal.AdopterId != null)
            {
               var adopterUser = await _userDomain.FindByIdAsync(animal.AdopterId.Value, token);

               if (adopterUser == default)
                    return NotFound(Constants.NotFoundProblemDetails);
            }

            await _domain.UpdateAsync(animalDb, animal, UserJwt.Id, token);
            return NoContent();
        }

        [HttpPut("{id}/mainpicture")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMainPicture([FromRoute] long id, IFormFile file, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);
            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            try
            {
                await _domain.UpdateMainPictureAsync(animalDb, file, UserJwt.Id, token);
                return NoContent();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(Constants.BadRequestProblemDetails(ex.Message));
            }
        }

        [HttpGet("{id}/additionalphotos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetAdditionalPhotosNames([FromRoute] long id, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);
            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            if (animalDb.AdditionalPhotosURL == null)
                return BadRequest(Constants.BadRequestProblemDetails("No additional photos uploaded yet."));

            var additionalPhotoUrls = _domain.GetAdditionalPhotoNames(animalDb);
            return Ok(additionalPhotoUrls);
        }

        [HttpGet("{id}/additionalphotos/{photoName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Image.Jpeg, Common.Constants.MediaTypeNamesImagePng)]
        public async Task<IActionResult> GetAdditionalPhoto([FromRoute] long id, [FromRoute] string photoName, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);
            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            if (animalDb.AdditionalPhotosURL == null)
                return BadRequest(Constants.BadRequestProblemDetails("No additional photos uploaded yet."));

            var path = _domain.GetAdditionalPhotoPath(animalDb.AdditionalPhotosURL, photoName);

            if (path == null)
                return NotFound(Constants.NotFoundProblemDetails);

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
            return NotFound(Constants.NotFoundProblemDetails);
        }

        [HttpPut("{id}/additionalphotos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAdditionalPhotos([FromRoute] long id, List<IFormFile> files, CancellationToken token)
        {
            if (files == null || files.Count == 0)
                return BadRequest(Constants.BadRequestProblemDetails("No images provided."));

            var animalDb = await _domain.FindByIdAsync(id, token);
            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);
            string? additionalPhotosGuid = null;

            if (animalDb.AdditionalPhotosURL != null)
            {
                var existingImages = _domain.GetAdditionalPhotoNames(animalDb);

                if (existingImages.Count + files.Count > Common.Constants.MaxAdditionalPhotos)
                {
                    return BadRequest(Constants.BadRequestProblemDetails($"You can upload a maximum of {Common.Constants.MaxAdditionalPhotos} additional photos."));
                }

                // Extract the guid from the existing AdditionalPhotosURL
                additionalPhotosGuid = animalDb.AdditionalPhotosURL
                    .TrimEnd('/', '\\')
                    .Split(['/', '\\'])
                    .Last();
            }
            else
            {
                if (files.Count > Common.Constants.MaxAdditionalPhotos)
                {
                    return BadRequest(Constants.BadRequestProblemDetails($"You can upload a maximum of {Common.Constants.MaxAdditionalPhotos} additional photos."));
                }
            }

            try 
            {
                await _domain.UploadAdditionalPhotosAsync(animalDb, files, additionalPhotosGuid, UserJwt.Id, token);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(Constants.BadRequestProblemDetails(ex.Message));
            }

            return NoContent();
        }

        [HttpDelete("{id}/additionalphotos/{photoName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdditionalPhoto([FromRoute] long id, [FromRoute] string photoName, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);
            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            if (animalDb.AdditionalPhotosURL == null)
                return BadRequest(Constants.BadRequestProblemDetails("No additional photos uploaded yet."));

            var path = _domain.GetAdditionalPhotoPath(animalDb.AdditionalPhotosURL, photoName);
            if (path == null)
                return NotFound(Constants.NotFoundProblemDetails);
            try
            {
                await _domain.DeleteAdditionalPhotoAsync(animalDb, photoName, UserJwt.Id, token);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to delete image");
                throw; // Let the global exception handler deal with this
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);

            if (animalDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            await _domain.DeleteAsync(animalDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
