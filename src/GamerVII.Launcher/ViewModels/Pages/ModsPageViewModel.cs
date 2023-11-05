using System.Windows.Input;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;

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

        #endregion

        #region Commands

        /// <summary>
        /// Command to navigate to the main page.
        /// </summary>
        public ICommand? GoToMainPageCommand { get; set; }

        #endregion

        #region Private properties

        private IGameClient? _selectedClient;

        #endregion
    }
}
