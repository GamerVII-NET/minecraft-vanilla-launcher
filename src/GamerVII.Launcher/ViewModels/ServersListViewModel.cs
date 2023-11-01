using System;
using System.Collections.Generic;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using GamerVII.Launcher.Services.Client;
using GamerVII.Launcher.Services.LocalStorage;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the list of game servers, derived from ViewModelBase.
/// </summary>
public class ServersListViewModel : ViewModelBase
{
    public delegate void SelectedServerHandler();
    public event SelectedServerHandler? SelectedServerChanged;

    public ObservableCollection<IGameClient> GameClients
    {
        get => _gameClients;
        set => this.RaiseAndSetIfChanged(ref _gameClients, value);
    }

    public IGameClient? SelectedClient
    {
        get => _selectedClient;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedClient, value);
            SelectedServerChanged?.Invoke();
        }
    }
    public ICommand AddClientCommand { get; set; } = null!;
    public ICommand RemoveClientCommand { get; set; } = null!;

    private readonly IGameClientService _gameClientService;
    private readonly ILocalStorageService _localStorageService;

    private ObservableCollection<IGameClient> _gameClients = null!;
    private IGameClient? _selectedClient;

    public ServersListViewModel(
        IGameClientService? gameClientService = null,
        ILocalStorageService? localStorageService = null
    )
    {
        _localStorageService = localStorageService
                               ?? Locator.Current.GetService<ILocalStorageService>()
                               ?? throw new Exception($"{nameof(ILocalStorageService)} not registered");

        _gameClientService = gameClientService
                             ?? Locator.Current.GetService<IGameClientService>()
                             ?? throw new Exception($"{nameof(IGameClientService)} not registered");

        RemoveClientCommand =
            ReactiveCommand.CreateFromTask(async (IGameClient gameClient) => RemoveClient(gameClient));

        LoadData();
    }

    private void RemoveClient(IGameClient gameClient)
    {
        _gameClients.Remove(gameClient);

        _localStorageService.SetAsync("Clients", _gameClients);

    }

    private async void LoadData()
    {
        var clients = await _gameClientService.GetClientsAsync();

        GameClients = new ObservableCollection<IGameClient>(clients);

        if (await _localStorageService.GetAsync<IEnumerable<GameClient>>("Clients") is { } localClients)
        {
            GameClients.AddRange(localClients);
        }

        SelectedClient = GameClients.FirstOrDefault();
    }
}
