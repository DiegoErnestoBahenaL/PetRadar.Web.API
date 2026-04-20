using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class MatchViewModel
    {
        public long Id { get; set; }
        public ReportViewModel LostReport { get; set; }
        public ReportViewModel StrayReport { get; set; }
        public double Score { get; set; }
        public double? DistanceInKM { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTimeOffset? ConfirmationDate { get; set; }

        public MatchViewModel() { }

        public MatchViewModel(MatchEntity entity)
        {
            Id = entity.Id;
            LostReport = new ReportViewModel(entity.LostReport);
            StrayReport = new ReportViewModel(entity.StrayReport);
            Score = entity.Score;
            DistanceInKM = entity.DistanceInKM;
            Status = entity.Status.ToString();
            Notes = entity.Notes;
            ConfirmationDate = entity.ConfirmationDate;
        }
        // This constructor is used when returning the view model fater creating a match,
        // where the lost and stray reports are not included in the match entity returned from the database,
        // so we need to pass them in separately.
        public MatchViewModel(MatchEntity entity, ReportEntity lostReport, ReportEntity strayReport)
        {
            Id = entity.Id;
            LostReport = new ReportViewModel(lostReport);
            StrayReport = new ReportViewModel(strayReport);
            Score = entity.Score;
            DistanceInKM = entity.DistanceInKM;
            Status = entity.Status.ToString();
            Notes = entity.Notes;
            ConfirmationDate = entity.ConfirmationDate;
        }

        public static List<MatchViewModel> FromList(List<MatchEntity> entities)
        {
            var viewModels = new List<MatchViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new MatchViewModel(entity));
            }
            return viewModels;
        }
    }
}
