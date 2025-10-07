using DeployService.Clients;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;

public class BackupManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupManager> _logger;
    private List<BackupDto> _backups = new();

    public BackupManager(IServiceProvider serviceProvider, ILogger<BackupManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task LoadAllBackupsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var backupApiClient = scope.ServiceProvider.GetRequiredService<BackupApiClient>();
        _backups = await backupApiClient.GetAllBackups();
        _logger.LogInformation("Loaded {Count} backups at startup.", _backups.Count);
    }

    public BackupDto? GetBackup(string baseline, string systemType)
    {
        string branch = baseline.Split('_')[0];
        return _backups.FirstOrDefault(b =>
            b.Name.Contains(branch, StringComparison.OrdinalIgnoreCase) &&
            b.Name.Contains(systemType, StringComparison.OrdinalIgnoreCase));
    }
}
