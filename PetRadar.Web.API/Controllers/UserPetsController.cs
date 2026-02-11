using Microsoft.AspNetCore.Mvc;
using PetRadar.Core;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.ViewModels;
using System.Net.Mime;

namespace PetRadar.Web.API.Controllers
{
    [ApiController]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] UserPetCreateModel pet, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(pet.UserId, token);

            if (user == default)
                return NotFound();


            var petDb = await _domain.CreateAsync(pet, 1, token);

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

            //Use JWT info
            await _domain.UpdateAsync(petdb, pet, 1, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var petdb = await _domain.FindByIdAsync(id, token);

            if (petdb == default)
                return NotFound();

            //Use JWT info
            await _domain.DeleteAsync(petdb, 1, token);
            return NoContent();
        }
    }
}
