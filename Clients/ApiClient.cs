using DeployService.Services;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using System.Net.Http.Json;


namespace DeployService.Clients
{
    /// <summary>
    /// Client for interacting with the API.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string requestApiRoute = "api/requests";
        private readonly string AdministratorApiRoute = "api/admins";
        private readonly string UserApiRoute = "api/users";
        private readonly string HostApiRoute = "api/hosts";
        private readonly string ScannerApiRoute = "api/scanners";
        private readonly string BaselineApiRoute = "api/baselines";

        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

        }

        /// <summary>
        /// Updates the request
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves the list of administrators based on adminType
        /// </summary>
        /// <param name="adminType"></param>
        /// <returns></returns>
        public async Task<List<AdministratorDto>> GetAdminstratorsAsync(string adminType)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{AdministratorApiRoute}/{adminType}");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Requests Updated successfully!");
                    var admins = await response.Content.ReadFromJsonAsync<List<AdministratorDto>>();
                    return admins ?? new List<AdministratorDto>();
                }
                else
                {
                    _logger.LogWarning($"Failed to get administrators: {response.StatusCode}");
                    return new List<AdministratorDto>();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new List<AdministratorDto>();
            }
        }

        /// <summary>
        /// Retrieves all requests
        /// </summary>
        /// <returns></returns>
        public async Task<List<RequestDto>> GetAllRequestsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(requestApiRoute);
                if (response.IsSuccessStatusCode)
                {
                    var requests = await response.Content.ReadFromJsonAsync<List<RequestDto>>();
                    return requests ?? new List<RequestDto>();
                }
                else
                {
                    _logger.LogWarning($"Failed to get requests: {response.StatusCode}");
                    return new List<RequestDto>();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return new List<RequestDto>();
            }
        }

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{UserApiRoute}/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }
                else
                {
                    _logger.LogWarning($"Failed to get user: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Retrieves a scanner by ID
        /// </summary>
        /// <param name="scannerId"></param>
        /// <returns></returns>
        public async Task<ScannerDto?> GetScannerByIdAsync(int scannerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ScannerApiRoute}/{scannerId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ScannerDto>();
                }
                else
                {
                    _logger.LogWarning($"Failed to get scanner: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Retrieves a host by ID
        /// </summary>
        /// <param name="hostId"></param>
        /// <returns></returns>
        public async Task<HostDto?> GetHostByIdAsync(int hostId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{HostApiRoute}/{hostId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<HostDto>();
                }
                else
                {
                    _logger.LogWarning($"Failed to get host: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// retrieves a baseline by ID
        /// </summary>
        /// <param name="baselineId"></param>
        /// <returns></returns>

        public async Task<BaseLineDto?> GetBaseLineByIdAsync(int baselineId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaselineApiRoute}/{baselineId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BaseLineDto>();
                }
                else
                {
                    _logger.LogWarning($"Failed to get baseline: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }



    }
}
