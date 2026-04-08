using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public class PetRadarProcessingHelperService : IPetRadarProcessingHelperService
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public PetRadarProcessingHelperService(IOptions<PetRadarCoreOptions> options, IHttpClientFactory httpClientFactory)
        {
            _baseUrl = options.Value.PetRadarProcessingBaseURL;

            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<ValidationResponse> ValidateCatOrDogAsync(Stream imageStream, string fileName, string contentType)
        {
            // Reiniciar la posición del stream si es posible
            if (imageStream.CanSeek)
            {
                imageStream.Position = 0;
            }

            // Copiamos a un MemoryStream para asegurar que la lectura del stream HTTP no falle
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var form = new MultipartFormDataContent();

            // Usar el MemoryStream en lugar del original de la llamada
            using var fileContent = new ByteArrayContent(memoryStream.ToArray());

            fileContent.Headers.ContentType =  new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

            form.Add(fileContent, "image", fileName);

            using var response = await _httpClient.PostAsync("/images/validatecatordog", form, CancellationToken.None);

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new BadHttpRequestException(body);
                }
                else
                {
                    throw new HttpRequestException($"Error calling PetRadarProcessing API: {response.StatusCode}, Body: {body}");
                }
            }

            return JsonConvert.DeserializeObject<ValidationResponse>(body);
            
        }
    }
}
