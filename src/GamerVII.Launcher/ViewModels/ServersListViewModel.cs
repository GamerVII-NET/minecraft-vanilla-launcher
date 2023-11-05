using System;
using System.Collections.Generic;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using GamerVII.Launcher.Services.Client;
using GamerVII.Launcher.Services.LocalStorage;

namespace GamerVII.Launcher.ViewModels
{
    /// <summary>
    /// View model class for the list of game servers, derived from ViewModelBase.
    /// </summary>
    public class ServersListViewModel : ViewModelBase
    {
        #region Events

        public delegate void SelectedServerHandler();

        /// <summary>
        /// Event raised when the selected game server is changed.
        /// </summary>
        public event SelectedServerHandler? SelectedServerChanged;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the collection of game clients.
        /// </summary>
        public ObservableCollection<IGameClient> GameClients
        {
            get => _gameClients;
            set => this.RaiseAndSetIfChanged(ref _gameClients, value);
        }

        /// <summary>
        /// Gets or sets the selected game client.
        /// </summary>
        public IGameClient? SelectedClient
        {
            get => _selectedClient;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedClient, value);
                SelectedServerChanged?.Invoke();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to add a new game client.
        /// </summary>
        public ICommand AddClientCommand { get; set; } = null!;

        /// <summary>
        /// Command to remove a game client.
        /// </summary>
        public ICommand RemoveClientCommand { get; set; }

        #endregion

        #region Private properties

        private readonly IGameClientService _gameClientService;
        private readonly ILocalStorageService _localStorageService;

        private ObservableCollection<IGameClient> _gameClients = null!;
        private IGameClient? _selectedClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ServersListViewModel class.
        /// </summary>
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
                ReactiveCommand.CreateFromTask((IGameClient gameClient) =>
                {
                    RemoveClient(gameClient);
                    return Task.CompletedTask;
                });

            LoadData();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Removes a game client from the collection.
        /// </summary>
        /// <param name="gameClient">The game client to remove.</param>
        private void RemoveClient(IGameClient gameClient)
        {
            _gameClients.Remove(gameClient);

            _localStorageService.SetAsync("Clients", _gameClients);
        }

        /// <summary>
        /// Loads data and initializes the view model.
        /// </summary>
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

        #endregion
    }
}
