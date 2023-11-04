using System.Diagnostics;
using System.Windows.Input;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;

namespace GamerVII.Launcher.ViewModels.Pages;

/// <summary>
/// View model class for the profile page, derived from PageViewModelBase.
/// </summary>
public class ProfilePageViewModel : PageViewModelBase
{

    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand GoToMainPageCommand { get; set; }
    public ICommand OpenLinkCommand { get; set; }

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
        OpenLinkCommand = ReactiveCommand.Create((string url) => OpenLink(url));
    }


    private void OpenLink(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
