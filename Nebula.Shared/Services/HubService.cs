using Nebula.Shared.Models;

namespace Nebula.Shared.Services;

[ServiceRegister]
public class HubService
{
    private readonly ConfigurationService _configurationService;
    private readonly RestService _restService;

    public Action<HubServerChangedEventArgs>? HubServerChangedEventArgs;
    public Action? HubServerLoaded;
    
    private bool _isUpdating = false;
    public HubService(ConfigurationService configurationService, RestService restService)
    {
        _configurationService = configurationService;
        _restService = restService;
        
        UpdateHub();
    }

    public async void UpdateHub()
    {
        if(_isUpdating) return;

        _isUpdating = true;
        
        
        HubServerChangedEventArgs?.Invoke(new HubServerChangedEventArgs([], HubServerChangeAction.Clear));
        
        foreach (var urlStr in _configurationService.GetConfigValue(CurrentConVar.Hub)!)
        {
            var servers = await _restService.GetAsyncDefault<List<ServerHubInfo>>(new Uri(urlStr), [], CancellationToken.None);
            HubServerChangedEventArgs?.Invoke(new HubServerChangedEventArgs(servers, HubServerChangeAction.Add));
        }
        
        _isUpdating = false;
        HubServerLoaded?.Invoke();
    }
    
}

public class HubServerChangedEventArgs : EventArgs
{
    public HubServerChangeAction Action;
    public List<ServerHubInfo> Items;

    public HubServerChangedEventArgs(List<ServerHubInfo> items, HubServerChangeAction action)
    {
        Items = items;
        Action = action;
    }
}

public enum HubServerChangeAction
{
    Add, Remove, Clear,
}