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
                return NotFound();

            return Ok(new AdoptionAnimalViewModel(animal));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] AdoptionAnimalCreateModel animal, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(animal.ShelterId, token);

            if (user == default)
                return NotFound();

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
                return NotFound();

            if (animal.AdopterId != null)
            {
               var adopterUser = await _userDomain.FindByIdAsync(animal.AdopterId.Value, token);

               if (adopterUser == default)
                    return NotFound("Usuario adoptante no encontrado");
            }

            await _domain.UpdateAsync(animalDb, animal, UserJwt.Id, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var animalDb = await _domain.FindByIdAsync(id, token);

            if (animalDb == default)
                return NotFound();

            await _domain.DeleteAsync(animalDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
