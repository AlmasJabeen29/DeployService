using DeployService.Clients;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;


namespace DeployService.Utilities
{
    public class RequestProcessor
    {

        private readonly ApiClient _ApiClient;

        public RequestProcessor(ApiClient ApiClient)
        {
            _ApiClient = ApiClient;
        }

        public async Task<IEnumerable<RequestDto>> GetNewRequestsAsync()
        {
            var allRequests = await _ApiClient.GetAllRequestsAsync();
            return allRequests.Where(r => r.Status == Status.New).ToList();
        }

        public async Task<(UserDto? User, HostDto? Host, ScannerDto? Scanner, BaseLineDto? Baseline)> RetrieveRequestDataAsync(RequestDto request)
        {
            var user = await _ApiClient.GetUserByIdAsync(request.UserId);
            var scanner = await _ApiClient.GetScannerByIdAsync(request.ScannerId);
            var host = await _ApiClient.GetHostByIdAsync(request.HostId);
            var baseline = await _ApiClient.GetBaseLineByIdAsync(request.BaselineId);

            return (user, host, scanner, baseline);
        }
    }
}
