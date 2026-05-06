using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Data.Repositories;

namespace PetRadar.Core.Helpers
{
    public class FcmPushNotificationService : IPushNotificationService
    {
        private readonly IUserRepository _userRepo;
        private readonly IReportRepository _reportRepo;
        private readonly ILogger<FcmPushNotificationService> _logger;

        public FcmPushNotificationService(IUserRepository userRepo, ILogger<FcmPushNotificationService> logger, IReportRepository reportRepo)
        {
            _userRepo = userRepo;
            _logger = logger;
            _reportRepo = reportRepo;
        }
        public async Task<bool> SendToUserAsync(long userId,  string title, string body, CancellationToken token)
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

        public async Task<bool> SendMessageNotificationToUserAsync(long userId, long messageToUserId, string title, string body, AdoptionAnimalEntity? adoptionAnimal, MatchEntity? match, CancellationToken token)
        {
            var user = await _userRepo.FindByIdAsync(userId, token);
            if (user == null || string.IsNullOrWhiteSpace(user.FcmToken))
            {
                _logger.LogInformation("Skipping push for user {UserId}: no FCM token registered", userId);
                return false;
            }

            var otherUser = await _userRepo.FindByIdAsync(messageToUserId, token);


            var data = new Dictionary<string, string> { };

            if (adoptionAnimal != null)
            {
                data = new Dictionary<string, string>
                {
                    { "extra_adoption_animal_id", adoptionAnimal.Id.ToString() },
                    { "extra_other_user_id", otherUser.Id.ToString() ?? "-1" },
                    { "extra_animal_name", adoptionAnimal.Name  },
                    { "extra_other_user_name", otherUser.Name }
                };
            }

            if (match != null)
            {

                var lostReport = await _reportRepo.FindByIdAsync(match.LostReportId, token);

                string animalName = lostReport.Species == PetSpeciesEnum.Dog ? "dog" : lostReport.Species == PetSpeciesEnum.Cat ? "cat" : string.Empty;

                data = new Dictionary<string, string>
                {
                    { "extra_match_id", match.Id.ToString() },
                    { "extra_other_user_id", otherUser.Id.ToString() ?? "-1" },
                    { "extra_lost_report_id", match.LostReportId.ToString() },
                    { "extra_stray_report_id", match.StrayReportId.ToString() },
                    { "extra_lost_pet_label", lostReport.ImageAnalysisResult.TopPredictedBreed },
                    { "extra_animal_name", animalName }
                };

            }

            var message = new Message
            {
                Token = user.FcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        // This should match the intent filter in the Android app
                        // to open the app when the notification is tapped
                        ClickAction = "OPEN_CHAT_ACTIVITY"
                    }
                },
                Data = data


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
