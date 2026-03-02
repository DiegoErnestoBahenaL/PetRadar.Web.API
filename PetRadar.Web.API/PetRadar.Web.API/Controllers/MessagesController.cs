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
    public class MessagesController : PetRadarController
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IMessageDomain _domain;

        public MessagesController(ILogger<MessagesController> logger, IMessageDomain domain)
        {
            _logger = logger;
            _domain = domain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MessageViewModel>>> Get(CancellationToken token)
        {
            var messages = await _domain.GetAllAsync(token);

            return Ok(MessageViewModel.FromList(messages));
        }

        [HttpGet("sender/{senderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MessageViewModel>>> GetBySenderId([FromRoute] long senderId, CancellationToken token)
        {
            var messages = await _domain.GetAllBySenderIdAsync(senderId, token);

            return Ok(MessageViewModel.FromList(messages));
        }

        [HttpGet("recipient/{recipientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MessageViewModel>>> GetByRecipientId([FromRoute] long recipientId, CancellationToken token)
        {
            var messages = await _domain.GetAllByRecipientIdAsync(recipientId, token);

            return Ok(MessageViewModel.FromList(messages));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<MessageViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var message = await _domain.FindByIdAsync(id, token);

            if (message == default)
                return NotFound();

            return Ok(new MessageViewModel(message));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] MessageCreateModel message, CancellationToken token)
        {
            var messageDb = await _domain.CreateAsync(message, UserJwt.Id, token);

            return CreatedAtAction(nameof(Get), new { id = messageDb.Id }, new MessageViewModel(messageDb));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] MessageUpdateModel message, CancellationToken token)
        {
            var messageDb = await _domain.FindByIdAsync(id, token);

            if (messageDb == default)
                return NotFound();

            await _domain.UpdateAsync(messageDb, message, UserJwt.Id, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var messageDb = await _domain.FindByIdAsync(id, token);

            if (messageDb == default)
                return NotFound();

            await _domain.DeleteAsync(messageDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
