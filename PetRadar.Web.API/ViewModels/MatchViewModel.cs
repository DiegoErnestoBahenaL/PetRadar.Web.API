using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class MatchViewModel
    {
        public long Id { get; set; }
        public long LostReportId { get; set; }
        public long StrayReportId { get; set; }
        public double Score { get; set; }
        public double? DistanceInKM { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTimeOffset? ConfirmationDate { get; set; }

        public MatchViewModel() { }

        public MatchViewModel(MatchEntity entity)
        {
            Id = entity.Id;
            LostReportId = entity.LostReportId;
            StrayReportId = entity.StrayReportId;
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
