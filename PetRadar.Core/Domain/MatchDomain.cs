using PetRadar.Common;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class MatchDomain : IMatchDomain
    {
        private readonly IMatchRepository _repo;
        private readonly IReportRepository _reportRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationDomain _notificationDomain;   
        private readonly IEmailHelperService _emailHelperService;
        private readonly ISystemConfigDomain _configService;
        public MatchDomain(IMatchRepository repo, IReportRepository reportRepo, 
            INotificationDomain notificationDomain, IEmailHelperService emailHelperService,
            IUserRepository userRepo, ISystemConfigDomain configService)
        {
            _repo = repo;
            _reportRepo = reportRepo;
            _notificationDomain = notificationDomain;
            _emailHelperService = emailHelperService;
            _userRepo = userRepo;
            _configService = configService;
        }

        public Task<List<MatchEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<MatchEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            return _repo.GetAllByUserIdAsync(userId, token);
        }

        public Task<List<MatchEntity>> GetAllByLostReportIdAsync(long lostReportId, CancellationToken token)
        {
            return _repo.GetAllByLostReportIdAsync(lostReportId, token);
        }

        public Task<List<MatchEntity>> GetAllByStrayReportIdAsync(long strayReportId, CancellationToken token)
        {
            return _repo.GetAllByStrayReportIdAsync(strayReportId, token);
        }

        public async Task<MatchEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var match = await _repo.FindByIdAsync(id, token);
            if (match == null)
                return default;

            return match;
        }

        public async Task<MatchEntity> CreateAsync(MatchCreateModel match, long createdByUserId, CancellationToken token)
        {
            var matchDb = new MatchEntity(
                match.LostReportId, match.StrayReportId, match.DistanceInKM,
                match.Notes, match.ConfirmationDate
            );

            matchDb.CreatedBy = createdByUserId;
            matchDb.CreatedAt = matchDb.UpdatedAt = DateTime.UtcNow;
            matchDb.IsActive = true;

            await _repo.AddAsync(matchDb);
            await _repo.SaveChangesAsync();
            return matchDb;
        }

        public async Task<int> UpdateAsync(MatchEntity matchDb, MatchUpdateModel match, long modifiedByUserId, CancellationToken token)
        {
            if (matchDb == default)
                throw new ArgumentNullException(nameof(matchDb));

            if (match.Score.HasValue)
                matchDb.Score = match.Score.Value;

            if (match.DistanceInKM.HasValue)
                matchDb.DistanceInKM = match.DistanceInKM.Value;

            if (match.Status.HasValue)
                matchDb.Status = match.Status.Value;

            if (!string.IsNullOrEmpty(match.Notes))
                matchDb.Notes = match.Notes;

            if (match.ConfirmationDate.HasValue)
                matchDb.ConfirmationDate = match.ConfirmationDate.Value;

            matchDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(matchDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteAsync(MatchEntity match, long modifiedByUserId, CancellationToken token)
        {
            if (match == default)
                throw new ArgumentNullException(nameof(match));

            match.IsActive = false;

            match.DeletedByUser(modifiedByUserId);
            _repo.Update(match);

            return await _repo.SaveChangesAsync();
        }

        public async Task<int> GenerateMatches(ReportEntity reportCreated, CancellationToken token)
        {
            if (reportCreated.ImageAnalysisResult == null)
            {
                return 0; // Cannot generate matches without image analysis results
            }

            List<ReportEntity> reports = [];

            if (reportCreated.ReportType == ReportTypeEnum.Lost)
            {
                reports = await _reportRepo.GetAllByStrayReportTypeAsync(reportCreated.Species, token);
            }
            else if (reportCreated.ReportType == ReportTypeEnum.Stray)
            {
                reports = await _reportRepo.GetAllByLostReportTypeAsync(reportCreated.Species, token);
            }
            else
            {
                return 0; // Invalid report type
            }

            if (reports == null || reports.Count == 0)
            {
                return 0;
            }

            var possibleMatches = reports.Where(x => x.ImageAnalysisResult != null);
            var config = await _configService.FindByKeyAsync(Constants.TopBreedPredictionsConfidenceConfigKey, token);

            decimal confidenceTreshold = 0.1m; // Default value
            if (config != null && decimal.TryParse(config.Value, System.Globalization.CultureInfo.InvariantCulture, out var parsedValue))
            {
                confidenceTreshold = parsedValue;
            }

            // For now, we will just look at the top predicted breed and breeds
            // with confidence > confidenceTreshold in the top predictions for both reports.
            possibleMatches = possibleMatches.Where(
                x => x.ImageAnalysisResult.TopPredictedBreed == reportCreated.ImageAnalysisResult.TopPredictedBreed ||
                reportCreated.ImageAnalysisResult.TopPredictions
                    .Any(y => y.Breed == x.ImageAnalysisResult.TopPredictedBreed && y.Confidence > confidenceTreshold) ||
                x.ImageAnalysisResult.TopPredictions    
                    .Any(y => y.Breed == reportCreated.ImageAnalysisResult.TopPredictedBreed && y.Confidence > confidenceTreshold) ||
                x.ImageAnalysisResult.TopPredictions
                    .Any(y => reportCreated.ImageAnalysisResult.TopPredictions
                        .Any(z => z.Breed == y.Breed && z.Confidence > confidenceTreshold))
            );

            // For now, we will also look at color matches.
            // This can be improved in the future by looking at the confidence of the color predictions and/or looking at other characteristics.
            possibleMatches = possibleMatches.Where(
                x => x.ImageAnalysisResult.Colors
                        .Select(c => c.Color)
                        .Intersect(reportCreated.ImageAnalysisResult.Colors.Select(c => c.Color))
                        .Any()
            ).ToList();


            var existingMatches = reportCreated.ReportType == ReportTypeEnum.Lost
                ? await _repo.GetAllByLostReportIdAsync(reportCreated.Id, token)
                : await _repo.GetAllByStrayReportIdAsync(reportCreated.Id, token);



            var existingByCounterpart = existingMatches
                .GroupBy(m => reportCreated.ReportType == ReportTypeEnum.Lost ? m.StrayReportId : m.LostReportId)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var possibleMatch in possibleMatches) 
            {
                double score = 0;

                if (possibleMatch.ImageAnalysisResult.TopPredictedBreed == reportCreated.ImageAnalysisResult.TopPredictedBreed)
                    score += 0.35;
                else if (reportCreated.ImageAnalysisResult.TopPredictions
                    .Any(y => y.Breed == possibleMatch.ImageAnalysisResult.TopPredictedBreed && y.Confidence > confidenceTreshold))
                    score += 0.3;
                else if
                    (possibleMatch.ImageAnalysisResult.TopPredictions
                    .Any(y => y.Breed == reportCreated.ImageAnalysisResult.TopPredictedBreed && y.Confidence > confidenceTreshold))
                    score += 0.25;
                else if (possibleMatch.ImageAnalysisResult.TopPredictions
                    .Any(y => reportCreated.ImageAnalysisResult.TopPredictions
                        .Any(z => z.Breed == y.Breed && z.Confidence > confidenceTreshold)))
                    score += 0.2;


                var sharedColors = possibleMatch.ImageAnalysisResult.Colors.Select(c => c.Color)
                    .Intersect(reportCreated.ImageAnalysisResult.Colors.Select(c => c.Color)).ToList();

                if (sharedColors.Count == possibleMatch.ImageAnalysisResult.Colors.Count
                    && sharedColors.Count == reportCreated.ImageAnalysisResult.Colors.Count)
                    score += 0.15;
                else if (sharedColors.Count > 0)
                    score += 0.1;

                
                if (possibleMatch.ImageAnalysisResult.Pattern == reportCreated.ImageAnalysisResult.Pattern)
                    score += 0.15;

                
                if (possibleMatch.Sex == reportCreated.Sex)
                    score += 0.1;

                if (possibleMatch.Size == reportCreated.Size)
                    score += 0.1;


                int distanceBetweenReports = DistanceBetweenReports(reportCreated, possibleMatch);

                if (distanceBetweenReports <= reportCreated.SearchRadiusMeters)
                    score += 0.1;

                long lostReportId = 0, strayReportId = 0;   

                if (reportCreated.ReportType == ReportTypeEnum.Lost)
                {
                    lostReportId = reportCreated.Id;
                    strayReportId = possibleMatch.Id;
                }
                else if (reportCreated.ReportType == ReportTypeEnum.Stray)
                {
                    lostReportId = possibleMatch.Id;
                    strayReportId = reportCreated.Id;
                }
                
             
                if ( existingByCounterpart.TryGetValue(possibleMatch.Id, out var existingMatch))
                {

                    if (existingMatch.Status == MatchStatusEnum.Pending)
                    {
                        existingMatch.Score = score;
                        existingMatch.DistanceInKM = distanceBetweenReports / 1000.0;
                        existingMatch.UpdatedByUser(Constants.SuperAdminId);

                        _repo.Update(existingMatch);
                    }
                }
                else
                {
                    var matchToCreate = new MatchEntity(lostReportId, strayReportId, distanceBetweenReports / 1000.0, null, null)
                    {
                        Score = score,
                        IsActive = true,
                    };

                    matchToCreate.CreatedByUser(Constants.SuperAdminId);
                    matchToCreate.UpdatedByUser(Constants.SuperAdminId);

                    await _repo.AddAsync(matchToCreate);




                }
            }
            if (possibleMatches.Any())
            {
                // Create a notification for the user about the new match
                await _notificationDomain.CreateAsync(
                     new NotificationCreateModel(
                         reportCreated.UserId,
                         NotificationTypeEnum.Match,
                         "Nuevo match encontrado!",
                         "Un nuevo match se ha generado a partir de tu reporte.",
                         null,
                         null),
                     Constants.SuperAdminId,
                 token);

                var user = await _userRepo.FindByIdAsync(reportCreated.UserId, token);

                await _emailHelperService.SendMatchFoundEmail(user);
            }

            return await _repo.SaveChangesAsync();
        }

        public int DistanceBetweenReports (ReportEntity report1, ReportEntity report2)
        {
            double distance = CalculateDistance(report1.Location.Y, report1.Location.X, report2.Location.Y, report2.Location.X);

            return (int)(distance * 1000); // Convert to meters
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Earth's radius in Kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in Kilometers
        }

        private double ToRadians(double angle) => (Math.PI / 180) * angle;
    }
}
