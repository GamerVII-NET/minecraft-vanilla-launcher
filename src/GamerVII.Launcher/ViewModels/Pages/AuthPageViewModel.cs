using System;
using System.Windows.Input;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.Auth;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages
{
    public class AuthPageViewModel : PageViewModelBase
    {
        #region Events

        /// <summary>
        /// Delegate for handling authorization events.
        /// </summary>
        public delegate void AuthorizeHandler(IUser user);

        /// <summary>
        /// Event raised when the authorization process is completed.
        /// </summary>
        public event AuthorizeHandler? Authorized;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the login string for authentication.
        /// </summary>
        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        /// <summary>
        /// Gets or sets the password string for authentication.
        /// </summary>
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is currently being authorized.
        /// </summary>
        public bool IsAuthorizing
        {
            get => _isAuthorizing;
            set => this.RaiseAndSetIfChanged(ref _isAuthorizing, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command that triggers the login process.
        /// </summary>
        public ICommand OnLoginCommand { get; }

        #endregion

        #region Private variables

        private readonly IAuthService _authService;
        private string _login = string.Empty;
        private string _password = string.Empty;
        private bool _isAuthorizing;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AuthPageViewModel class.
        /// </summary>
        /// <param name="authService">An optional IAuthService implementation for user authentication.</param>
        public AuthPageViewModel(IAuthService? authService = null)
        {
            _authService = authService
                           ?? Locator.Current.GetService<IAuthService>()
                           ?? throw new Exception("AuthService not registered");

            var canAuthorize = this.WhenAnyValue(
                x => x.Login, x => x.Password,
                (login, password) =>
                    !string.IsNullOrWhiteSpace(login)
            );

            OnLoginCommand = ReactiveCommand.Create(OnLogin, canAuthorize);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the login process.
        /// </summary>
        private async void OnLogin()
        {
            IsAuthorizing = true;

            var user = await _authService.OnLogin(Login, Password);

            if (user.IsLogin)
            {
                Authorized?.Invoke(user);
            }

            IsAuthorizing = false;
        }

        #endregion
    }
}
