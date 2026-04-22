namespace PetRadar.Core.Helpers
{
    public interface IPushNotificationService
    {
        Task<bool> SendToUserAsync(long userId, string title, string body, CancellationToken token);
    }
}
