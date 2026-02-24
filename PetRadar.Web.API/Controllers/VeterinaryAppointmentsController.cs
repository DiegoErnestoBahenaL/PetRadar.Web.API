using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class VeterinaryAppointmentsController : PetRadarController
    {
        private readonly ILogger<VeterinaryAppointmentsController> _logger;
        private readonly IVeterinaryAppointmentDomain _domain;
        private readonly IUserPetDomain _userPetDomain;
        private readonly IUserDomain _userDomain;

        public VeterinaryAppointmentsController(ILogger<VeterinaryAppointmentsController> logger, IVeterinaryAppointmentDomain domain, IUserPetDomain userPetDomain, IUserDomain userDomain)
        {
            _logger = logger;
            _domain = domain;
            _userPetDomain = userPetDomain;
            _userDomain = userDomain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<VeterinaryAppointmentViewModel>>> Get(CancellationToken token)
        {
            var appointments = await _domain.GetAllAsync(token);

            return Ok(VeterinaryAppointmentViewModel.FromList(appointments));
        }

        [HttpGet("pet/{petId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<VeterinaryAppointmentViewModel>>> GetByPetId([FromRoute] long petId, CancellationToken token)
        {
            var pet = await _userPetDomain.FindByIdAsync(petId, token);

            if (pet == default)
                return NotFound();

            var appointments = await _domain.GetAllByPetIdAsync(pet.Id, token);

            return Ok(VeterinaryAppointmentViewModel.FromList(appointments));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<VeterinaryAppointmentViewModel>>> GetByUserId([FromRoute] long userId, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(userId, token);

            if (user == default)
                return NotFound();

            var appointments = await _domain.GetAllByUserIdAsync(user.Id, token);

            return Ok(VeterinaryAppointmentViewModel.FromList(appointments));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<VeterinaryAppointmentViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var appointment = await _domain.FindByIdAsync(id, token);

            if (appointment == default)
                return NotFound();

            return Ok(new VeterinaryAppointmentViewModel(appointment));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] VeterinaryAppointmentCreateModel appointment, CancellationToken token)
        {
            var pet = await _userPetDomain.FindByIdAsync(appointment.PetId, token);

            if (pet == default)
                return NotFound();

            var appointmentDb = await _domain.CreateAsync(appointment, 1, token);

            return CreatedAtAction(nameof(Get), new { id = appointmentDb.Id }, new VeterinaryAppointmentViewModel(appointmentDb));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] VeterinaryAppointmentUpdateModel appointment, CancellationToken token)
        {
            var appointmentDb = await _domain.FindByIdAsync(id, token);

            if (appointmentDb == default)
                return NotFound();

            //Use JWT info
            await _domain.UpdateAsync(appointmentDb, appointment, 1, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var appointmentDb = await _domain.FindByIdAsync(id, token);

            if (appointmentDb == default)
                return NotFound();

            //Use JWT info
            await _domain.DeleteAsync(appointmentDb, 1, token);
            return NoContent();
        }
    }
}
