using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Helpers;
using System.IO.Compression;
using System.Net.Mime;

namespace PetRadar.Web.API.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(RoleEnum.SuperAdmin) + "," + nameof(RoleEnum.Admin))]
    [Route("api/[controller]")]
    public class SystemConfigController : PetRadarController
    {
        private readonly IFileHelperService _fileHelper;
        private readonly ILogger<SystemConfigController> _logger;
        private readonly string _connectionString;

        public SystemConfigController(IFileHelperService fileHelper, ILogger<SystemConfigController> logger, IConfiguration configuration)
        {
            _fileHelper = fileHelper;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        [HttpGet("imagesbackup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Zip)]
        public IActionResult GetImagesBackup(CancellationToken token)
        {
            string imagesDirectory = _fileHelper.GetImagesDirectoryPath();

            try
            {
                var stream = new MemoryStream();
                ZipFile.CreateFromDirectory(imagesDirectory, stream);

                // Reset stream position to the beginning before returning
                stream.Position = 0;

                // Return the stream directly, letting the framework automatically dispose it when the response is finished
                return File(stream, MediaTypeNames.Application.Zip, "images_backup.zip");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating images backup");
            }

            return NotFound(Constants.NotFoundProblemDetails);
        }

        [HttpGet("dbbackup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Zip)]

        public async Task<IActionResult> GetDbBackup(CancellationToken token)
        {
            string backupDirectory = _fileHelper.GetDbBackupsDirectoryPath();
            string backupFilePath = Path.Combine(backupDirectory, $"db_backup_{DateTime.UtcNow:yyyyMMddHHmmss}");

            try
            {
                // Clear old backup files before creating a new one
                Array.ForEach(Directory.GetFiles(backupDirectory), System.IO.File.Delete);

                int exitCode = await BackupHelper.BackupDataBase(_connectionString, backupFilePath, token);
                if (exitCode != 0)
                {
                    _logger.LogError("pg_dump process failed with exit code: {ExitCode}", exitCode);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Database backup process failed.");
                }

                var stream = new MemoryStream();

                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    archive.CreateEntryFromFile(backupFilePath, Path.GetFileName(backupFilePath));
                }

                // Reset stream position to the beginning before returning
                stream.Position = 0;
                // Return the stream directly, letting the framework automatically dispose it when the response is finished
                return File(stream, MediaTypeNames.Application.Zip, "db_backup.zip");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating database backup");
            }

            return NotFound(Constants.NotFoundProblemDetails);
        }
    }
}
