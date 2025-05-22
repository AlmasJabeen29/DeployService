using DeployService.Services;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using System.Net.Http.Json;


namespace DeployService.Clients
{
    public class RequestApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string requestApiRoute = "api/requests";
        private readonly ILogger<RequestApiClient> _logger;

        public RequestApiClient(HttpClient httpClient, ILogger<RequestApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

        }

        public async Task UpdateRequestAsync(RequestDto requestDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{requestApiRoute}/{requestDto.Id}", requestDto);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Requests Updated successfully!");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }
}
