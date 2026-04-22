using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using PetRadar.Core.Data.Repositories;

namespace PetRadar.Core.Helpers
{
    public class FcmPushNotificationService : IPushNotificationService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<FcmPushNotificationService> _logger;

        public FcmPushNotificationService(IUserRepository userRepo, ILogger<FcmPushNotificationService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<bool> SendToUserAsync(long userId, string title, string body, CancellationToken token)
        {
            var user = await _userRepo.FindByIdAsync(userId, token);
            if (user == null || string.IsNullOrWhiteSpace(user.FcmToken))
            {
                _logger.LogInformation("Skipping push for user {UserId}: no FCM token registered", userId);
                return false;
            }

            var message = new Message
            {
                Token = user.FcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message, token);
                _logger.LogInformation("Sent FCM push to user {UserId} (messageId={MessageId})", userId, messageId);
                return true;
            }
            catch (FirebaseMessagingException ex) when (
                ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                _logger.LogWarning(ex, "FCM token for user {UserId} is invalid/unregistered, clearing it", userId);
                user.FcmToken = null;
                _userRepo.Update(user);
                await _userRepo.SaveChangesAsync();
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send FCM push to user {UserId}", userId);
                return false;
            }
        }
    }
}
