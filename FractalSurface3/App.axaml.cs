using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using FractalSurface3.Helpers.Configuration;
using FractalSurface3.Helpers.Configuration.Json;
using FractalSurface3.Services.Projects;
using FractalSurface3.Services.Settings;
using FractalSurface3.ViewModels;
using FractalSurface3.Views;
using Material.Icons;
using Material.Icons.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace FractalSurface3;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        // Default logic doesn't auto detect windows theme anymore in designer
        // to stop light mode, force here
        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.Closing += MainWindowOnClosing;
            
            var services = new ServiceCollection();
            
            services.AddSingleton<ISettingsService>(x =>
            {
                x.CreateScope();
                return new SettingsService();
            });
            
            services.AddSingleton<IStorageProvider>(x =>
            {
                x.CreateScope();
                return desktop.MainWindow.StorageProvider;
            });
            
            services.AddSingleton<IProjectsService>(x =>
            {
                x.CreateScope();
                return new ProjectsService();
            });
            
            Services = services.BuildServiceProvider();
            
            desktop.MainWindow.DataContext = new MainWindowViewModel();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void MainWindowOnClosing(object? sender, WindowClosingEventArgs e)
    {
        var projectsService = Services?.GetService<IProjectsService>();

        if (projectsService?.IsChanged == true)
        {
            e.Cancel = true;

            var titlePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children =
                {
                    new MaterialIcon
                    {
                        Foreground = new SolidColorBrush(Colors.Yellow),
                        Height = 20,
                        Width = 20,
                        Kind = MaterialIconKind.Warning
                    },
                    new TextBlock
                    {
                        Text = "Сохранение изменений"
                    }
                }
            };

            var confirmDialog = new ContentDialog
            {
                Title = titlePanel,
                Content = "У вас остались текущие несохранённые изменения проекта. Хотите сохранить?",
                PrimaryButtonText = "Сохранить",
                SecondaryButtonText = "Не сохранять",
                CloseButtonText = "Отменить"
            };
            confirmDialog.Resources["ContentDialogMaxWidth"] = 1080;

            var result = await confirmDialog.ShowAsync();
            
            if (result is ContentDialogResult.Primary) await projectsService.SaveProjectFileAsync();
            
            if (result is ContentDialogResult.None) return;
            
            Environment.Exit(0);
        }
    }


    public new static App? Current => Application.Current as App;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }
}