using System.Windows.Input;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the profile page, derived from PageViewModelBase.
/// </summary>
public class ProfilePageViewModel : PageViewModelBase
{

    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand GoToMainPageCommand { get; set; }

    public IUser User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }

    private IUser _user;

    /// <summary>
    /// Initializes a new instance of the ProfilePageViewModel class.
    /// </summary>
    public ProfilePageViewModel()
    {
        // Add any initialization logic or data loading here if required.
    }
}
