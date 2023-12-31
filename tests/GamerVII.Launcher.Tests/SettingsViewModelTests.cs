using GamerVII.Launcher.Extensions;
using GamerVII.Launcher.ViewModels;
using GamerVII.Launcher.ViewModels.Pages;
using ReactiveUI;

namespace GamerVII.Launcher.Tests;

public class SettingsTests
{
    private MainWindowViewModel _mainViewModel;
    private SettingsPageViewModel _settingsViewModel;

    [SetUp]
    public void Setup()
    {
        ServiceRegister.RegisterServices();

        _mainViewModel = new MainWindowViewModel();

        _settingsViewModel = _mainViewModel.GetPageViewModelByType<SettingsPageViewModel>() as SettingsPageViewModel
                             ?? throw new Exception($"{nameof(SettingsPageViewModel)} not found");

        _settingsViewModel.GoToMainPageCommand ??= ReactiveCommand.CreateFromTask(_mainViewModel.SaveSettings);
    }

    [Test]
    public void DefaultWindowNotEmptyTest()
    {
        _mainViewModel.OpenPage<SettingsPageViewModel>();

        Assert.That(_mainViewModel.CurrentPage, Is.Not.Null);
    }

    [Test]
    public void ResetPageEmptyTest()
    {
        _mainViewModel.ResetPage();

        Assert.That(_mainViewModel.CurrentPage, Is.Null);
    }

    [Test]
    public void CheckDefaultDataSettingsTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_settingsViewModel, Is.Not.Null);
            Assert.That(_settingsViewModel.MemorySize, Is.Not.Zero);
            Assert.That(_settingsViewModel.WindowWidth, Is.Not.Zero);
            Assert.That(_settingsViewModel.WindowHeight, Is.Not.Zero);
        });
    }

    [Test]
    public void SaveSettingsTest()
    {
        _settingsViewModel.WindowWidth = 100;
        _settingsViewModel.WindowHeight = 100;
        _settingsViewModel.MemorySize = 256;

        _settingsViewModel.GoToMainPageCommand?.Execute(null);

        Assert.Multiple(() =>
        {
            Assert.That(_settingsViewModel.WindowWidth, Is.EqualTo(100));
            Assert.That(_settingsViewModel.WindowHeight, Is.EqualTo(100));
            Assert.That(_settingsViewModel.MemorySize, Is.EqualTo(256));
        });
    }
}
