
using System.Net.Http.Json;
using BackupDto = NumarisConnectt.Application.DataTransferObjects.RetrievalDtos.BackupDto;


namespace DeployService.Clients
{
    internal class BackupApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _backupApiRoute = "api/backups";
        private readonly ILogger<BackupApiClient> _logger;

        public BackupApiClient(HttpClient httpClient, ILogger<BackupApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<BackupDto>> GetAllBackups()
        {
            try
            {
                var response = await _httpClient.GetAsync(_backupApiRoute);
                if (response.IsSuccessStatusCode)
                {
                    var backups = await response.Content.ReadFromJsonAsync<List<BackupDto>>();
                    _logger.LogInformation($"Retrieved {backups?.Count} backups successfully!");
                    return backups ?? new List<NumarisConnectt.Application.DataTransferObjects.RetrievalDtos.BackupDto>();
                }

                _logger.LogWarning($"Failed to retrieve backups. Status code: {response.StatusCode}");
                return new List<BackupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving backups.");
                return new List<BackupDto>();
            }
        }

    }
}

