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
    public class NotificationsController : PetRadarController
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationDomain _domain;
        private readonly IUserDomain _userDomain;

        public NotificationsController(ILogger<NotificationsController> logger, INotificationDomain domain, IUserDomain userDomain)
        {
            _logger = logger;
            _domain = domain;
            _userDomain = userDomain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<NotificationViewModel>>> Get(CancellationToken token)
        {
            var notifications = await _domain.GetAllAsync(token);

            return Ok(NotificationViewModel.FromList(notifications));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<NotificationViewModel>>> GetByUserId([FromRoute] long userId, CancellationToken token)
        {
            var notifications = await _domain.GetAllByUserIdAsync(userId, token);

            return Ok(NotificationViewModel.FromList(notifications));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<NotificationViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var notification = await _domain.FindByIdAsync(id, token);

            if (notification == default)
                return NotFound();

            return Ok(new NotificationViewModel(notification));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] NotificationCreateModel notification, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(notification.UserId.Value, token);

            if (user == default)
                return NotFound();

            var notificationDb = await _domain.CreateAsync(notification, UserJwt.Id, token);

            return CreatedAtAction(nameof(Get), new { id = notificationDb.Id }, new NotificationViewModel(notificationDb));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] NotificationUpdateModel notification, CancellationToken token)
        {
            var notificationDb = await _domain.FindByIdAsync(id, token);

            if (notificationDb == default)
                return NotFound();

            await _domain.UpdateAsync(notificationDb, notification, UserJwt.Id, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var notificationDb = await _domain.FindByIdAsync(id, token);

            if (notificationDb == default)
                return NotFound();

            await _domain.DeleteAsync(notificationDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
