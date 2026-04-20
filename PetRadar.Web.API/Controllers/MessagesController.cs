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
        private readonly IUserDomain _userDomain;
        private readonly IMatchDomain _matchDomain;
        private readonly IAdoptionAnimalDomain _adoptionAnimalDomain;

        public MessagesController(ILogger<MessagesController> logger, IMessageDomain domain, IUserDomain userDomain, IMatchDomain matchDomain, IAdoptionAnimalDomain adoptionAnimalDomain)
        {
            _logger = logger;
            _domain = domain;
            _userDomain = userDomain;
            _matchDomain = matchDomain;
            _adoptionAnimalDomain = adoptionAnimalDomain;
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

        [HttpGet("match/{matchId}/conversation/{recipientId}/{senderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]

        public async Task<ActionResult<IList<MessageViewModel>>> GetByMatchIdAndRecipientIdAndSenderId([FromRoute] long matchId, [FromRoute] long recipientId, [FromRoute] long senderId, CancellationToken token)
        {
            var match = await _matchDomain.FindByIdAsync(matchId, token);

            if (match == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var recipient = await _userDomain.FindByIdAsync(recipientId, token);
            
            if (recipient == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var sender = await _userDomain.FindByIdAsync(senderId, token);
            
            if (sender == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var messages = await _domain.GetAllByMatchIdConversationAsync(matchId, recipientId, senderId, token);

            if (messages == default)
                return NotFound(Constants.NotFoundProblemDetails);
            return Ok(MessageViewModel.FromList(messages));
        }

        [HttpGet("adoptionAnimal/{adoptionAnimalId}/conversation/{recipientId}/{senderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MessageViewModel>>> GetByAdoptionAnimalIdAndRecipientIdAndSenderId([FromRoute] long adoptionAnimalId, [FromRoute] long recipientId, [FromRoute] long senderId, CancellationToken token)
        {
            var adoptionAnimal = await _adoptionAnimalDomain.FindByIdAsync(adoptionAnimalId, token);

            if (adoptionAnimal == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var recipient = await _userDomain.FindByIdAsync(recipientId, token);
            
            if (recipient == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var sender = await _userDomain.FindByIdAsync(senderId, token);
            
            if (sender == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var messages = await _domain.GetAllByAdoptionAnimalIdConversationAsync(adoptionAnimalId, recipientId, senderId, token);

            if (messages == default)
                return NotFound(Constants.NotFoundProblemDetails);
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
                return NotFound(Constants.NotFoundProblemDetails);

            return Ok(new MessageViewModel(message));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] MessageCreateModel message, CancellationToken token)
        {

            var sender = await _userDomain.FindByIdAsync(message.SenderId, token);
            
            if (sender == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var recipient = await _userDomain.FindByIdAsync(message.RecipientId, token);

            if (recipient == default)
                return NotFound(Constants.NotFoundProblemDetails);


            if (message.MatchId != null)
            {
                var match = await _matchDomain.FindByIdAsync(message.MatchId.Value, token);
                if (match == default)
                    return NotFound(Constants.NotFoundProblemDetails);
            }

            if (message.AdoptionAnimalId != null)
            {
                var adoptionAnimal = await _adoptionAnimalDomain.FindByIdAsync(message.AdoptionAnimalId.Value, token);
                if (adoptionAnimal == default)
                    return NotFound(Constants.NotFoundProblemDetails);
            }

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
                return NotFound(Constants.NotFoundProblemDetails);

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
                return NotFound(Constants.NotFoundProblemDetails);

            await _domain.DeleteAsync(messageDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
