using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers;
using PetRadar.Core.Helpers.PetRadarProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class ReportDomain : IReportDomain
    {
        private readonly IReportRepository _repo;
        private readonly IFileHelperService _fileHelperService;
        private readonly ILogger<ReportDomain> _logger;
        private readonly IPetRadarProcessingHelperService _processingHelperService;

        public ReportDomain(IReportRepository repo, IFileHelperService fileHelperService, ILogger<ReportDomain> logger, IPetRadarProcessingHelperService processingHelperService)
        {
            _repo = repo;
            _fileHelperService = fileHelperService;
            _logger = logger;
            _processingHelperService = processingHelperService;
        }

        public Task<List<ReportEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<ReportEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            return _repo.GetAllByUserIdAsync(userId, token);
        }

        public async Task<ReportEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var report = await _repo.FindByIdAsync(id, token);
            if (report == null)
                return default;

            report.Views++;

            _repo.Update(report);

            await _repo.SaveChangesAsync();

            return report;
        }

        public Task<string?> GetMainPicturePath(ReportEntity reportDb, CancellationToken token)
        {
            if (reportDb.PhotoURL == null)
                return Task.FromResult<string?>(null);

            string path = _fileHelperService.GetImagePath(reportDb.PhotoURL);
            return Task.FromResult<string?>(path);
        }

        public async Task<int> UpdateMainPictureAsync(ReportEntity reportDb, IFormFile file, long modifiedByUserId, CancellationToken token)
        {
            await using var imageStream = file.OpenReadStream();

            try
            {
                var validationResult = await _processingHelperService.ValidateCatOrDogAsync(imageStream, file.FileName, file.ContentType);

                if (reportDb.Species != PetSpeciesEnum.NotSet && 
                    reportDb.Species.ToString().ToLower() != validationResult.DetectedClass.ToLower())
                {
                    throw new BadHttpRequestException("The image detected class is different from the registered species.");
                }

                if (reportDb.Species == PetSpeciesEnum.NotSet)
                {

                    if (validationResult.DetectedClass.ToLower() == "cat")
                    {
                        reportDb.Species = PetSpeciesEnum.Cat;
                    }
                    else if (validationResult.DetectedClass.ToLower() == "dog")
                    {
                        reportDb.Species = PetSpeciesEnum.Dog;
                    }
                }           
            }
            catch (BadHttpRequestException ex) 
            { 
                throw new BadHttpRequestException($"Image validation failed: {ex.Message}");
            }

            if (reportDb.PhotoURL != null)
            {
                _fileHelperService.DeleteImage(reportDb.PhotoURL, _logger);

                reportDb.PhotoURL = null;
            }

            string relativePath = await _fileHelperService.SaveImage(file);

            reportDb.PhotoURL = relativePath;

            reportDb.UpdatedByUser(modifiedByUserId);

            _repo.Update(reportDb);

            int result = await _repo.SaveChangesAsync();

            try
            {
                var characteristicsResult = await _processingHelperService.GetAnimalCharacteristicsAsync(reportDb.Species, imageStream, file.FileName, file.ContentType);

                if (characteristicsResult != null)
                {
                    reportDb.Breed = characteristicsResult.TopPredictedBreed;
                    reportDb.UpdatedByUser(modifiedByUserId);
                    _repo.Update(reportDb);
                    await _repo.SaveChangesAsync();
                }
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException($"Image characteristics extraction failed: {ex.Message}");
            }

            return result;
        }

        public async Task<ReportEntity> CreateAsync(ReportCreateModel report, long createdByUserId, CancellationToken token)
        {
            var location = new Point(report.Longitude.Value, report.Latitude.Value) { SRID = 4326 };

            //Defaulting to Stray for now, as the ReportType won't be sent from the client when creating a report.
            //This can be updated later when the client is sending the ReportType.
            var reportDb = new ReportEntity(
                report.UserId.Value, report.UserPetId,
                report.Species.Value, report.Breed, report.Color,
                report.Sex, report.Size, report.ApproximateAge,
                report.Weight, report.Description, report.IsNeutered, report.ReportType.Value,
                report.ReportStatus, report.HasCollar, report.HasTag, report.IncidentDate,
                location, report.AddressText, report.UseAlternateContact, report.ContactName,
                report.ContactPhone, report.ContactEmail, report.RewardAmount
            );

            //If the report type is different than Lost, default the species to NotSet,
            //and the report type to stray
            if (report.ReportType.Value != ReportTypeEnum.Lost)
            {
                reportDb.Species = PetSpeciesEnum.NotSet;
                reportDb.ReportType = ReportTypeEnum.Stray;
            }

            reportDb.SearchRadiusMeters = report.SearchRadiusMeters;
            reportDb.OffersReward = report.OffersReward;
            reportDb.CreatedBy = createdByUserId;
            reportDb.CreatedAt = reportDb.UpdatedAt = DateTime.UtcNow;
            reportDb.IsActive = true;

            await _repo.AddAsync(reportDb);
            await _repo.SaveChangesAsync();
            return reportDb;
        }

        public async Task<int> UpdateAsync(ReportEntity reportDb, ReportUpdateModel report, long modifiedByUserId, CancellationToken token)
        {
            if (reportDb == default)
                throw new ArgumentNullException(nameof(reportDb));

            if (report.Species.HasValue)
                reportDb.Species = report.Species.Value;

            if (!string.IsNullOrEmpty(report.Breed))
                reportDb.Breed = report.Breed;

            if (!string.IsNullOrEmpty(report.Color))
                reportDb.Color = report.Color;

            if (report.Sex.HasValue)
                reportDb.Sex = report.Sex.Value;

            if (report.Size.HasValue)
                reportDb.Size = report.Size.Value;

            if (report.ApproximateAge.HasValue)
                reportDb.ApproximateAge = report.ApproximateAge.Value;

            if (report.Weight.HasValue)
                reportDb.Weight = report.Weight.Value;

            if (!string.IsNullOrEmpty(report.Description))
                reportDb.Description = report.Description;

            if (report.IsNeutered.HasValue)
                reportDb.IsNeutered = report.IsNeutered.Value;

            if (report.ReportType.HasValue)
                reportDb.ReportType = report.ReportType.Value;

            if (report.ReportStatus.HasValue)
                reportDb.ReportStatus = report.ReportStatus.Value;

            if (report.HasCollar.HasValue)
                reportDb.HasCollar = report.HasCollar.Value;

            if (report.HasTag.HasValue)
                reportDb.HasTag = report.HasTag.Value;

            if (report.IncidentDate.HasValue)
                reportDb.IncidentDate = report.IncidentDate.Value;

            if (report.Latitude.HasValue && report.Longitude.HasValue)
                reportDb.Location = new Point(report.Longitude.Value, report.Latitude.Value) { SRID = 4326 };

            if (!string.IsNullOrEmpty(report.AddressText))
                reportDb.AddressText = report.AddressText;

            if (report.SearchRadiusMeters.HasValue)
                reportDb.SearchRadiusMeters = report.SearchRadiusMeters.Value;

            if (report.UseAlternateContact.HasValue)
                reportDb.UseAlternateContact = report.UseAlternateContact.Value;

            if (!string.IsNullOrEmpty(report.ContactName))
                reportDb.ContactName = report.ContactName;

            if (!string.IsNullOrEmpty(report.ContactPhone))
                reportDb.ContactPhone = report.ContactPhone;

            if (!string.IsNullOrEmpty(report.ContactEmail))
                reportDb.ContactEmail = report.ContactEmail;

            if (report.OffersReward.HasValue)
                reportDb.OffersReward = report.OffersReward.Value;

            if (report.RewardAmount.HasValue)
                reportDb.RewardAmount = report.RewardAmount.Value;

            reportDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(reportDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public List<string> GetAdditionalPhotoNames(ReportEntity reportDb)
        {
            if (reportDb.AdditionalPhotosURL == null)
                return [];

            return _fileHelperService.GetGalleryImageNames(reportDb.AdditionalPhotosURL);
        }

        public string? GetAdditionalPhotoPath(string relativePath, string imageName)
        {
            // Delegates to the file helper which validates the filename (blocks path traversal)
            // and verifies the image exists on disk.
            return _fileHelperService.GetGalleryImagePath(relativePath, imageName);
        }

        public async Task<int> UploadAdditionalPhotosAsync(ReportEntity reportDb, List<IFormFile> files, string? guid, long modifiedByUserId, CancellationToken token)
        {
            try
            {
                foreach (var file in files)
                {
                    await using var imageStream = file.OpenReadStream();

                    var validationResult = await _processingHelperService.ValidateCatOrDogAsync(imageStream, file.FileName, file.ContentType);

                    if (reportDb.Species != Data.Entities.Enums.PetSpeciesEnum.NotSet &&
                        reportDb.Species.ToString().ToLower() != validationResult.DetectedClass.ToLower())
                    {
                        throw new BadHttpRequestException("The image detected class is different from the registered species.");
                    }
                }
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException($"Image validation failed: {ex.Message}");
            }

            string? relativePath = await _fileHelperService.SaveImagesInGallery(files, guid);

            if (relativePath != null)
            {
                reportDb.AdditionalPhotosURL = relativePath;
            }

            reportDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(reportDb);
            int result = await _repo.SaveChangesAsync();
            return result;
        }

        public async Task<int> DeleteAdditionalPhotoAsync(ReportEntity reportDb, string photoName, long modifiedByUserId, CancellationToken token)
        {
            if (reportDb == default)
                throw new ArgumentNullException(nameof(reportDb));

            if (string.IsNullOrEmpty(reportDb.AdditionalPhotosURL))
                throw new InvalidOperationException("Report has no additional photos gallery.");

            var path = GetAdditionalPhotoPath(reportDb.AdditionalPhotosURL, photoName);
            if (path == null)
                throw new FileNotFoundException($"Additional photo '{photoName}' not found.");

            System.IO.File.Delete(path);

            reportDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(reportDb);
            return await _repo.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(ReportEntity report, long modifiedByUserId, CancellationToken token)
        {
            if (report == default)
                throw new ArgumentNullException(nameof(report));

            report.IsActive = false;

            report.DeletedByUser(modifiedByUserId);
            _repo.Update(report);

            return await _repo.SaveChangesAsync();
        }
    }
}
