using System.Diagnostics;
using System.Windows.Input;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;

namespace GamerVII.Launcher.ViewModels.Pages
{
    /// <summary>
    /// View model class for the profile page, derived from PageViewModelBase.
    /// </summary>
    public class ProfilePageViewModel : PageViewModelBase
    {
        #region Commands

        /// <summary>
        /// Command to navigate to the main page.
        /// </summary>
        public ICommand? GoToMainPageCommand { get; set; }

        /// <summary>
        /// Command to open a web link.
        /// </summary>
        public ICommand? OpenLinkCommand { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the user associated with the profile.
        /// </summary>
        public IUser User
        {
            get => _user ?? new User();
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        #endregion

        #region Private variables

        private IUser? _user;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ProfilePageViewModel class.
        /// </summary>
        public ProfilePageViewModel()
        {
            OpenLinkCommand = ReactiveCommand.Create((string url) => OpenLink(url));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Opens a web link using the default system browser.
        /// </summary>
        private void OpenLink(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        #endregion
    }
}
