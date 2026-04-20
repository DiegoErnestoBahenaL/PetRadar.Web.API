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
    public class MatchesController : PetRadarController
    {
        private readonly ILogger<MatchesController> _logger;
        private readonly IMatchDomain _domain;
        private readonly IReportDomain _reportDomain;
        private readonly IUserDomain _userDomain;

        public MatchesController(ILogger<MatchesController> logger, IMatchDomain domain, IReportDomain reportDomain, IUserDomain userDomain)
        {
            _logger = logger;
            _domain = domain;
            _userDomain = userDomain;
            _reportDomain = reportDomain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MatchViewModel>>> Get(CancellationToken token)
        {
            var matches = await _domain.GetAllAsync(token);

            return Ok(MatchViewModel.FromList(matches));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]

        public async Task<ActionResult<IList<MatchViewModel>>> GetByUserId([FromRoute] long userId, CancellationToken token)
        {

            var user = await _userDomain.FindByIdAsync(userId, token);

            if (user == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var matches = await _domain.GetAllByUserIdAsync(userId, token);

            return Ok(MatchViewModel.FromList(matches));
        }

        [HttpGet("lost-report/{lostReportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MatchViewModel>>> GetByLostReportId([FromRoute] long lostReportId, CancellationToken token)
        {
            var matches = await _domain.GetAllByLostReportIdAsync(lostReportId, token);

            return Ok(MatchViewModel.FromList(matches));
        }

        [HttpGet("stray-report/{strayReportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<MatchViewModel>>> GetByStrayReportId([FromRoute] long strayReportId, CancellationToken token)
        {
            var matches = await _domain.GetAllByStrayReportIdAsync(strayReportId, token);

            return Ok(MatchViewModel.FromList(matches));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<MatchViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var match = await _domain.FindByIdAsync(id, token);

            if (match == default)
                return NotFound(Constants.NotFoundProblemDetails);

            return Ok(new MatchViewModel(match));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] MatchCreateModel match, CancellationToken token)
        {
            var lostReport = await _reportDomain.FindByIdAsync(match.LostReportId, token);

            if (lostReport == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var strayReport = await _reportDomain.FindByIdAsync(match.StrayReportId, token);

            if (strayReport == default)
                return NotFound(Constants.NotFoundProblemDetails);

            var matchDb = await _domain.CreateAsync(match, UserJwt.Id, token);

            return CreatedAtAction(nameof(Get), new { id = matchDb.Id }, new MatchViewModel(matchDb, lostReport, strayReport));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] MatchUpdateModel match, CancellationToken token)
        {
            var matchDb = await _domain.FindByIdAsync(id, token);

            if (matchDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            await _domain.UpdateAsync(matchDb, match, UserJwt.Id, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var matchDb = await _domain.FindByIdAsync(id, token);

            if (matchDb == default)
                return NotFound(Constants.NotFoundProblemDetails);

            await _domain.DeleteAsync(matchDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
