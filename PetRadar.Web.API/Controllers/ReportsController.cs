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
    public class ReportsController : PetRadarController
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IReportDomain _domain;
        private readonly IUserDomain _userDomain;

        public ReportsController(ILogger<ReportsController> logger, IReportDomain domain, IUserDomain userDomain)
        {
            _logger = logger;
            _domain = domain;
            _userDomain = userDomain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<ReportViewModel>>> Get(CancellationToken token)
        {
            var reports = await _domain.GetAllAsync(token);

            return Ok(ReportViewModel.FromList(reports));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IList<ReportViewModel>>> GetByUserId([FromRoute] long userId, CancellationToken token)
        {
            var reports = await _domain.GetAllByUserIdAsync(userId, token);

            return Ok(ReportViewModel.FromList(reports));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ReportViewModel>> Get([FromRoute] long id, CancellationToken token)
        {
            var report = await _domain.FindByIdAsync(id, token);

            if (report == default)
                return NotFound();

            return Ok(new ReportViewModel(report));
        }

        [HttpGet("{id}/mainpicture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Image.Jpeg, Common.Constants.MediaTypeNamesImagePng)]
        public async Task<IActionResult> GetMainPicture([FromRoute] long id, CancellationToken token)
        {
            var report = await _domain.FindByIdAsync(id, token);
            if (report == default)
                return NotFound();

            var path = await _domain.GetMainPicturePath(report, token);
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

        [HttpPut("{id}/mainpicture")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMainPicture([FromRoute] long id, IFormFile file, CancellationToken token)
        {
            var reportDb = await _domain.FindByIdAsync(id, token);
            if (reportDb == default)
                return NotFound();

            await _domain.UpdateMainPictureAsync(reportDb, file, UserJwt.Id, token);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Post([FromBody] ReportCreateModel report, CancellationToken token)
        {
            var user = await _userDomain.FindByIdAsync(report.UserId.Value, token);

            if (user == default)
                return NotFound();

            var reportDb = await _domain.CreateAsync(report, UserJwt.Id, token);

            return CreatedAtAction(nameof(Get), new { id = reportDb.Id }, new ReportViewModel(reportDb));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] ReportUpdateModel report, CancellationToken token)
        {
            var reportDb = await _domain.FindByIdAsync(id, token);

            if (reportDb == default)
                return NotFound();

            await _domain.UpdateAsync(reportDb, report, UserJwt.Id, token);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken token)
        {
            var reportDb = await _domain.FindByIdAsync(id, token);

            if (reportDb == default)
                return NotFound();

            await _domain.DeleteAsync(reportDb, UserJwt.Id, token);
            return NoContent();
        }
    }
}
