using System;
using RestSharp; // RestSharp v112.1.0
using RestSharp.Authenticators;
using System.Threading;
using System.Threading.Tasks;
using PetRadar.Core.Data.Entities;
using Microsoft.Extensions.Options;

namespace PetRadar.Core.Helpers
{
    public class EmailHelperService : IEmailHelperService
    {

        private readonly string _apiKey;
        private readonly string _petRadarBaseURL;

        public EmailHelperService(IOptions<PetRadarCoreOptions> options)
        {
            _apiKey = options.Value.MailGunAPIKey;
            _petRadarBaseURL = options.Value.BaseURL;
        }
        public async Task<RestResponse> SendVerificationEmail(UserEntity user, string token)
        {

            string emailVerificationLink = $"{_petRadarBaseURL}api/gate/Login/VerifyEmail/{token}";

            var options = new RestClientOptions("https://api.mailgun.net")
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            };

            var client = new RestClient(options);
            var request = new RestRequest("/v3/petradar-qa.org/messages", Method.Post);
            request.AlwaysMultipartFormData = false;

            request.AddParameter("from", "PetRadar <noreply@petradar-qa.org>");
            request.AddParameter("to", user.Email);
            request.AddParameter("subject", $"Verificación de email para {user.Name} {user.LastName}");
            request.AddParameter("text", $"Para verificar tu email en el sistema PetRadar, haz click en el siguiente enlace:\n {emailVerificationLink}");
            return await client.ExecuteAsync(request);

        }
    }
}
