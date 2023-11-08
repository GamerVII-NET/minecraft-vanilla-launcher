using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Mods;
using GamerVII.Launcher.Models.Mods.Modrinth;
using GamerVII.Launcher.Services.Mods;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages
{
    /// <summary>
    /// View model class for the profile page, derived from PageViewModelBase.
    /// </summary>
    public class ModsPageViewModel : PageViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the selected game client.
        /// </summary>
        public IGameClient? SelectClient
        {
            get => _selectedClient;
            set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
        }

        /// <summary>
        /// Gets or sets the mods list
        /// </summary>
        public ObservableCollection<IMod> Mods
        {
            get => _mods;
            set => this.RaiseAndSetIfChanged(ref _mods, value);
        }

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        /// <summary>
        /// A flag that indicates whether the system is currently busy with a task.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        #endregion

        #region Constructors

        public ModsPageViewModel(IModsService? modsService = null)
        {
            _modsService = modsService
                           ?? Locator.Current.GetService<IModsService>()
                           ?? throw new Exception($"{nameof(IModsService)} not registered!");

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);

        }


        #region Private properties

        private IGameClient? _selectedClient;
        private ObservableCollection<IMod> _mods = new();
        private readonly IModsService _modsService;
        private IFilter<IFilterItem> _filter = new ModrinthFilter<IFilterItem>();
        private string _searchText;
        private bool _isBusy;
        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to navigate to the main page.
        /// </summary>
        public ICommand? GoToMainPageCommand { get; set; }

        #endregion

        #region Private methods

        private async Task LoadDataMods()
        {
            var mods = await _modsService.GetModsListAsync(_filter);

            Mods = new ObservableCollection<IMod>(mods);
        }


        private async void DoSearch(string s)
        {
            IsBusy = true;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                _filter.Query = SearchText;
            }

            await LoadDataMods();

            IsBusy = false;
        }


        #endregion
    }
}
