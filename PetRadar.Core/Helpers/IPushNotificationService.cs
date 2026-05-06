using PetRadar.Core.Data.Entities;

namespace PetRadar.Core.Helpers
{
    public interface IPushNotificationService
    {
        Task<bool> SendToUserAsync(long userId, string title, string body, CancellationToken token);

        Task<bool> SendMessageNotificationToUserAsync(long userId, long messageToUserId, string title, string body, AdoptionAnimalEntity? adoptionAnimal, MatchEntity? match, CancellationToken token);

    }
}
