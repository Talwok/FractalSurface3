using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using FractalSurface3.Services.Settings;
using Material.Icons;
using Material.Icons.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FractalSurface3.ViewModels.Dialogs;

public class SettingsDialogViewModel : ViewModelBase
{
    private readonly IStorageProvider? _storageProvider;

    public SettingsDialogViewModel()
    {
        _storageProvider = App.Current?.Services?.GetService<IStorageProvider>();
        
        SettingsSvc ??= App.Current?.Services?.GetService<ISettingsService>();
        
        SelectReportsTemplateFolderCommand = ReactiveCommand.CreateFromTask(SelectReportsTemplateFolderAsync);
        SelectReportsOutputFolderCommand = ReactiveCommand.CreateFromTask(SelectReportsOutputFolderAsync);
        
        SettingsSvc?.Load();
    }

    private async Task SelectReportsOutputFolderAsync()
    {
        if (_storageProvider == null) return;
        if (SettingsSvc == null) return;

        var folders = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку для отчётов",
            SuggestedStartLocation = null,
            AllowMultiple = false
        });
        
        if(folders.Count == 0) return;
        
        SettingsSvc.AppSettings.ReportSettings.OutputDirectory = folders[0].Path.LocalPath;
    }

    private async Task SelectReportsTemplateFolderAsync()
    {
        if (_storageProvider == null) return;
        if (SettingsSvc == null) return;

        var folders = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку для шаблонов отчётов",
            SuggestedStartLocation = null,
            AllowMultiple = false
        });
        
        if(folders.Count == 0) return;
        
        SettingsSvc.AppSettings.ReportSettings.TemplatesDirectory = folders[0].Path.LocalPath;
    }
    
    private async Task ClearSettingsAsync()
    {
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
                    Text = "Сброс настроек"
                }
            }
        };
        
        var confirmDialog = new ContentDialog
        {
            Title = titlePanel,
            Content = "Вы уверены, что хотите сбросить все настройки до изначальных?",
            PrimaryButtonText = "Сбросить",
            CloseButtonText = "Отменить"
        };

        var result = await confirmDialog.ShowAsync();

        if (result is ContentDialogResult.Primary)
        {
            SettingsSvc?.Clear();
        }
    }
    
    public ISettingsService? SettingsSvc { get; }

    public ICommand SelectReportsTemplateFolderCommand { get; }
    public ICommand SelectReportsOutputFolderCommand { get; }
    
    public void ApplyDialog(ContentDialog dialog)
    {
        dialog.PrimaryButtonCommand = ReactiveCommand.Create(() => SettingsSvc?.Save());
        dialog.SecondaryButtonCommand = ReactiveCommand.CreateFromTask(ClearSettingsAsync);
        dialog.CloseButtonCommand = ReactiveCommand.Create(() => SettingsSvc?.Load());
        
        dialog.Closing += (_, args) =>
        {
            args.Cancel = args.Result is ContentDialogResult.Primary or ContentDialogResult.Secondary;
        };
        
        SettingsSvc?.WhenAnyValue(x => x.IsChanged)
            .Subscribe(isChanged => dialog.IsPrimaryButtonEnabled = isChanged);
    }
}