using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain;
using PetRadar.Core.Domain.Models;
using System.Net.Mime;

namespace PetRadar.Web.API.Controllers
{
    [ApiController]
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
        public async Task<ActionResult<IList<UserEntity>>> Get(CancellationToken token)
        {

            var users = await _domain.GetAllAsync(token);

            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<UserEntity>> Get([FromRoute] long id, CancellationToken token)
        {
            var user = await _domain.FindByIdAsync(id, token);

            if (user == default)
                return NotFound();

            return Ok(user);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] UserCreateModel user, CancellationToken token)
        {
            //Use the JWT info
            var userdb = await _domain.CreateAsync(user, 1, token);
            return CreatedAtAction(nameof(Get), new { id = userdb.Id }, userdb);
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
            await _domain.UpdateAsync(userdb, user, 1, token);
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
            await _domain.DeleteAsync(userdb, 1, token);
            return NoContent();
        }

    }
}
