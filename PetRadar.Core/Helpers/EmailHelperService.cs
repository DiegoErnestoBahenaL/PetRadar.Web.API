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
        private readonly RestClientOptions _restClientOptions;
        private readonly string _restRequestResource = "/v3/petradar-qa.org/messages";

        public EmailHelperService(IOptions<PetRadarCoreOptions> options)
        {
            _apiKey = options.Value.MailGunAPIKey;
            _petRadarBaseURL = options.Value.BaseURL;
            _restClientOptions = new RestClientOptions("https://api.mailgun.net")
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            };
        }
        public async Task<RestResponse> SendVerificationEmail(UserEntity user, string token)
        {

            string emailVerificationLink = $"{_petRadarBaseURL}api/gate/Login/VerifyEmail/{token}";


            var client = new RestClient(_restClientOptions);
            var request = new RestRequest(_restRequestResource, Method.Post);
            request.AlwaysMultipartFormData = false;

            request.AddParameter("from", "PetRadar <noreply@petradar-qa.org>");
            request.AddParameter("to", user.Email);
            request.AddParameter("subject", $"Verificación de email para {user.Name} {user.LastName}");
            request.AddParameter("text", $"Para verificar tu email en el sistema PetRadar, haz click en el siguiente enlace:\n {emailVerificationLink}");
            return await client.ExecuteAsync(request);

        }
        public async Task<RestResponse> SendMatchFoundEmail(UserEntity user)
        {
            var client = new RestClient(_restClientOptions);
            var request = new RestRequest(_restRequestResource, Method.Post);
            request.AlwaysMultipartFormData = false;

            request.AddParameter("from", "PetRadar <noreply@petradar-qa.org>");
            request.AddParameter("to", user.Email);
            request.AddParameter("subject", $"Actualización de reporte para {user.Name} {user.LastName}");
            request.AddParameter("text", $"El reporte que subiste ha hecho match con otro reporte en nuestro sistema. Ingresa a la aplicación para revisarlo.");
            return await client.ExecuteAsync(request);
        }

        public async Task<RestResponse> SendRecoverPasswordEmail(UserEntity user, string password)
        {
            var client = new RestClient(_restClientOptions);
            var request = new RestRequest(_restRequestResource, Method.Post);
            request.AlwaysMultipartFormData = false;
            request.AddParameter("from", "PetRadar <noreply@petradar-qa.org>");
            request.AddParameter("to", user.Email);
            request.AddParameter("subject", $"Recuperación de contraseña para {user.Name} {user.LastName}");
            request.AddParameter("text", $"Tu nueva contraseña es: {password}\nCambia tu contraseña después de iniciar sesión.");
            return await client.ExecuteAsync(request);
        }
    }
}
