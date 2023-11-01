using System.Windows.Input;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;

namespace GamerVII.Launcher.ViewModels.Pages;

/// <summary>
/// View model class for the profile page, derived from PageViewModelBase.
/// </summary>
public class ModsPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand GoToMainPageCommand { get; set; }

    public IGameClient SelectClient
    {
        get => _selectedClient;
        set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
    }

    private IGameClient _selectedClient;

    /// <summary>
    /// Initializes a new instance of the ProfilePageViewModel class.
    /// </summary>
    public ModsPageViewModel()
    {
        // Add any initialization logic or data loading here if required.
    }
}
