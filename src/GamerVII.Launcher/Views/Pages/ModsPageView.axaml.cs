using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GamerVII.Launcher.ViewModels.Pages;

namespace GamerVII.Launcher.Views.Pages;

public partial class ModsPageView : UserControl
{
    private CompositeDisposable _disposables = new CompositeDisposable();

    private CompositeDisposable _scrollViewerDisposables;

    private double _verticalHeightMax = 0.0;

    public ModsPageView()
    {
        InitializeComponent();

        var listBox = this.FindControl<ListBox>("ViewModsListBox") ?? throw new Exception("listBox not found");

        listBox.GetObservable(ListBox.ScrollProperty)!
            .OfType<ScrollViewer>()
            .Take(1)
            .Subscribe(sv =>
            {
                _scrollViewerDisposables?.Dispose();
                _scrollViewerDisposables = new CompositeDisposable();

                sv.GetObservable(ScrollViewer.ScrollBarMaximumProperty)
                    .Subscribe(newMax => _verticalHeightMax = newMax.Y)
                    .DisposeWith(_scrollViewerDisposables);


                sv.GetObservable(ScrollViewer.OffsetProperty)
                    .Subscribe(offset =>
                    {
                        if (offset.Y <= Double.Epsilon)
                        {
                            Console.WriteLine("At Top");
                        }

                        var delta = Math.Abs(_verticalHeightMax - offset.Y);
                        if (delta <= Double.Epsilon)
                        {
                            Console.WriteLine("At Bottom");
                            var vm = DataContext as ModsPageViewModel;
                            vm?.LoadNextElementsCommand.Execute(null);
                        }
                    }).DisposeWith(_disposables);
            }).DisposeWith(_disposables);
    }
}

