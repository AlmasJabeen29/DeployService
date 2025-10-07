using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;

namespace DeployService.Utilities
{
    public class RequestProcessor
    {

        private readonly IUserService _userService;
        private readonly IScannerService _scannerService;
        private readonly IHostService _hostService;
        private readonly IBaseLineService _baselineService;
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestProcessor> _logger;

        public RequestProcessor(
            IUserService userService,
            IScannerService scannerService,
            IHostService hostService,
            IBaseLineService baselineService,
            IRequestService requestService,
            ILogger<RequestProcessor> logger)
        {
            _userService = userService;
            _scannerService = scannerService;
            _hostService = hostService;
            _baselineService = baselineService;
            _requestService = requestService;
            _logger = logger;
        }

        public async Task<IEnumerable<RequestDto>> GetNewRequestsAsync()
        {
            try
            {
                var result = await _requestService.GetAllRequestsAsync();
                return result.IsSuccess
                    ? result.Value.Where(r => r.Status == Status.New).ToList()
                    : new List<RequestDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetNewRequestsAsync: {ex.Message}");
                return new List<RequestDto>();
            }
        }

        public async Task<(UserDto? User, HostDto? Host, ScannerDto? Scanner, BaseLineDto? Baseline, UserDto? Assignee)> RetrieveRequestDataAsync(RequestDto request)
        {
            try
            {
                var user = (await _userService.GetUserByIdAsync(request.UserId)).Value;
                var scanner = (await _scannerService.GetScannerByIdAsync(request.ScannerId)).Value;
                var host = (await _hostService.GetHostByIdAsync(request.HostId)).Value;
                var baseline = (await _baselineService.GetBaseLineByIdAsync(request.BaselineId)).Value;
                if (request.AssigneeId == null)
                {
                    return (user, host, scanner, baseline, null);
                }
                var assignee = (await _userService.GetUserByIdAsync((int)request.AssigneeId)).Value;

                return (user, host, scanner, baseline, assignee);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RetrieveRequestDataAsync: {ex.Message}");
                return (null, null, null, null, null);
            }
        }
    }
}