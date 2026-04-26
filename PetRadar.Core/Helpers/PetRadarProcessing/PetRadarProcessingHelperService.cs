using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PetRadar.Core.Data.Entities.Enums;
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
            // Reiniciar la posici�n del stream si es posible
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

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

            form.Add(fileContent, "image", fileName);

            using var response = await _httpClient.PostAsync("/images/validatecatordog", form, CancellationToken.None);

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {

                var errorResponse = JsonConvert.DeserializeObject<HttpExceptionResponse>(body);


                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {

                    throw new BadHttpRequestException(errorResponse.Detail);
                }
                else
                {
                    throw new HttpRequestException($"Error calling PetRadarProcessing API: {response.StatusCode}, Body: {errorResponse.Detail}");
                }
            }

            return JsonConvert.DeserializeObject<ValidationResponse>(body);

        }

        public async Task<CharacteristicsResponse> GetAnimalCharacteristicsAsync(PetSpeciesEnum species, Stream imageStream, string fileName, string contentType)
        {

            if (species == PetSpeciesEnum.NotSet)
            {
                throw new BadHttpRequestException("Species must be set to either Dog or Cat.");
            }

            // Reiniciar la posici�n del stream si es posible
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
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
            form.Add(fileContent, "image", fileName);
            using var response = await _httpClient.PostAsync($"/images/{species.ToString().ToLower()}/extractcharacteristics", form, CancellationToken.None);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<HttpExceptionResponse>(body);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {

                    throw new BadHttpRequestException(errorResponse.Detail);
                }
                else
                {
                    throw new HttpRequestException($"Error calling PetRadarProcessing API: {response.StatusCode}, Body: {errorResponse.Detail}");
                }

            }

            var characteristics = JsonConvert.DeserializeObject<CharacteristicsResponse>(body);

            // Translate breed labels returned by the EfficientNet classifier into
            // Spanish (Mexican context) before handing the response back.
            return BreedTranslationHelper.TranslateCharacteristicsResponse(species, characteristics);
        }

        public async Task<ConfigsResponse> GetConfigs()
        {
            using var response = await _httpClient.GetAsync("/configs", CancellationToken.None);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<HttpExceptionResponse>(body);
                throw new HttpRequestException($"Error calling PetRadarProcessing API: {response.StatusCode}, Body: {errorResponse.Detail}");
            }
            return JsonConvert.DeserializeObject<ConfigsResponse>(body);
        }


        public async Task<UpdateConfigsResponse> UpdateConfigs(string yoloConfThreshold, string topKBreedPrediction)
        {
            using var response = await _httpClient
                .PutAsync($"/configs?yolo_conf_threshold={yoloConfThreshold}&&top_k_breed_predictions={topKBreedPrediction}", new StringContent(string.Empty), CancellationToken.None);
           
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<HttpExceptionResponse>(body);
                throw new HttpRequestException($"Error calling PetRadarProcessing API: {response.StatusCode}, Body: {errorResponse.Detail}");
            }
            var updateResponse = JsonConvert.DeserializeObject<UpdateConfigsResponse>(await response.Content.ReadAsStringAsync());
            
            return updateResponse;
        }

    }
}
