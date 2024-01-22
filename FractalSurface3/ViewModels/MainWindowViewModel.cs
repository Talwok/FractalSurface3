using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FractalSurface3.Services;
using FractalSurface3.Services.Projects;
using FractalSurface3.ViewModels.Dialogs;
using Material.Icons;
using Material.Icons.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        ProjectsSvc ??= App.Current?.Services?.GetService<IProjectsService>();

        CreateProjectFileCommand = ReactiveCommand.CreateFromTask(ProjectsSvc.CreateProjectFileAsync);
        OpenProjectFileCommand = ReactiveCommand.CreateFromTask(ProjectsSvc.OpenProjectFileAsync);
        SaveProjectFileAsCommand = ReactiveCommand.CreateFromTask(ProjectsSvc.SaveProjectFileAsAsync);
        
        ImportFilesCommand = ReactiveCommand.CreateFromTask<bool>(ImportFiles);
        
        // SelectAllFilesCommand = ReactiveCommand.Create(ToggleAllFiles,
        //     FilesSvc?.ToggleFileCanExecute);
        // UnselectAllFilesCommand = ReactiveCommand.Create(UntoggleAllFiles,
        //     FilesSvc?.ToggleFileCanExecute);
        // RemoveFilesCommand = ReactiveCommand.Create(RemoveFiles, 
        //     FilesSvc?.RemoveFileCanExecute);

        MoveTabSelectionLeftCommand = ReactiveCommand.Create(() =>
        {
            // if(FilesSvc?.TabViewImageFile == null) return;
            //
            // var index = FilesSvc?.ToggledFiles.IndexOf(FilesSvc?.TabViewImageFile);
            //
            // if (index.HasValue)
            // {
            //     index--;
            // }
            //
            // if (index == -1)
            // {
            //     index = FilesSvc?.ToggledFiles.Count - 1;
            // }
            //
            // var element = FilesSvc?.ToggledFiles.ElementAt(index.Value);
            //
            // if (element != null)
            // {
            //     FilesSvc.TabViewImageFile = element;
            // }
        });
        MoveTabSelectionRightCommand = ReactiveCommand.Create(() =>
        {
            // if(FilesSvc?.TabViewImageFile == null) return;
            //
            // var index = FilesSvc?.ToggledFiles.IndexOf(FilesSvc?.TabViewImageFile);
            //
            // if (index.HasValue)
            // {
            //     index++;
            // }
            //
            // if (index + 1 > FilesSvc?.ToggledFiles.Count)
            // {
            //     index = 0;
            // }
            //
            // var element = FilesSvc?.ToggledFiles.ElementAt(index.Value);
            //
            // if (element != null)
            // {
            //     FilesSvc.TabViewImageFile = element;
            // }
        });
        
        ClearSearchTextCommand = ReactiveCommand.Create(() =>
        {
            // if (FilesSvc != null) 
            //     FilesSvc.SearchText = string.Empty;
        });

        ShowSettingsDialogCommand = ReactiveCommand.CreateFromTask(ShowSettingsDialog);
        ShowReportDialogCommand = ReactiveCommand.CreateFromTask(ShowReportDialog);
        ShowAnalysisDialogCommand = ReactiveCommand.CreateFromTask(ShowAnalysisDialog);
        ShowFilterDialogCommand = ReactiveCommand.CreateFromTask(ShowFilterDialog);

        ExitCommand = ReactiveCommand.Create(() =>
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow?.Close();   
            }
        });
    }
    
    private async Task ShowFilterDialog()
    {
        var titlePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new MaterialIcon
                {
                    Height = 20,
                    Width = 20,
                    Kind = MaterialIconKind.Filter
                },
                new TextBlock
                {
                    Text = "Фильтр"
                }
            }
        };
        var dialog = new ContentDialog
        {
            Content = new AnalysisDialogViewModel(),
            DefaultButton = ContentDialogButton.Close,
            IsPrimaryButtonEnabled = true,
            PrimaryButtonText = "Сохранить",
            CloseButtonText = "Закрыть",
            Title = titlePanel,
        };
        dialog.Resources["ContentDialogMaxWidth"] = 1080;

        var result = await dialog.ShowAsync();
    }

    private async Task ShowReportDialog()
    {
        var titlePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new MaterialIcon
                {
                    Height = 20,
                    Width = 20,
                    Kind = MaterialIconKind.Printer
                },
                new TextBlock
                {
                    Text = "Отчёт"
                }
            }
        };
        var dialog = new ContentDialog
        {
            Content = new AnalysisDialogViewModel(),
            DefaultButton = ContentDialogButton.Close,
            IsPrimaryButtonEnabled = true,
            PrimaryButtonText = "Сохранить",
            CloseButtonText = "Закрыть",
            Title = titlePanel,
        };
        dialog.Resources["ContentDialogMaxWidth"] = 1080;

        var result = await dialog.ShowAsync();
    }

    private async Task ShowAnalysisDialog()
    {
        var titlePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new MaterialIcon
                {
                    Height = 20,
                    Width = 20,
                    Kind = MaterialIconKind.Analytics
                },
                new TextBlock
                {
                    Text = "Анализ"
                }
            }
        };
        
        var dialog = new ContentDialog
        {
            Content = new AnalysisDialogViewModel(),
            DefaultButton = ContentDialogButton.Close,
            PrimaryButtonText = "Сохранить",
            CloseButtonText = "Закрыть",
            Title = titlePanel
        };
        dialog.Resources["ContentDialogMaxWidth"] = 1080;

        

        var result = await dialog.ShowAsync();
    }

    private async Task ShowSettingsDialog()
    {
        var titlePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new MaterialIcon
                {
                    Height = 20,
                    Width = 20,
                    Kind = MaterialIconKind.Cog
                },
                new TextBlock
                {
                    Text = "Настройки"
                }
            }
        };

        var content = new SettingsDialogViewModel();
        
        var dialog = new ContentDialog
        {
            Content = content,
            DefaultButton = ContentDialogButton.Close,
            PrimaryButtonText = "Сохранить",
            SecondaryButtonText = "Сбросить",
            CloseButtonText = "Закрыть",
            Title = titlePanel
        };
        dialog.Resources["ContentDialogMaxWidth"] = 1080;

        content.ApplyDialog(dialog);
        
        var result = await dialog.ShowAsync();
    }

    #region Command Methods
    private async Task OpenNewFilesAsync()
    {
        //if (FilesSvc == null) return;
        
        //await FilesSvc.OpenNewFilesAsync();
    }

    private async Task SaveFilesAsync()
    {
        //if (FilesSvc == null) return;

        //await FilesSvc.SaveFilesAsync(false);
    }
    
    private void RemoveFiles()
    {
        //if (FilesSvc == null) return;

        //FilesSvc.RemoveToggledFiles();
    }

    private void ToggleAllFiles()
    {
        //if (FilesSvc == null) return;

        //FilesSvc.ToggleAllFiles();
    }
    
    private void UntoggleAllFiles()
    {
        //if (FilesSvc == null) return;

        //FilesSvc.UntoggleAllFiles();
    }

    private async Task ImportFiles(bool toggleImported)
    {
        //if (FilesSvc == null) return;

        //await FilesSvc.ImportFilesAsync(toggleImported);
    }
    #endregion
    
    #region Commands
    public ICommand CreateProjectFileCommand { get; private set; }
    public ICommand OpenProjectFileCommand { get; private set; }
    public ICommand SaveProjectFileAsCommand { get; private set; }
    
    public ICommand RemoveFilesCommand { get; private set; }
    public ICommand ImportFilesCommand { get; private set; }
    public ICommand SelectAllFilesCommand { get; private set; }
    public ICommand UnselectAllFilesCommand { get; private set; }
    public ICommand ClearSearchTextCommand { get; private set; }
    public ICommand MoveTabSelectionRightCommand { get; private set; }
    public ICommand MoveTabSelectionLeftCommand { get; private set; }
    
    public ICommand ShowAnalysisDialogCommand { get; private set; }
    public ICommand ShowSettingsDialogCommand { get; private set; }
    public ICommand ShowFilterDialogCommand { get; private set; }
    public ICommand ShowReportDialogCommand { get; private set; }
    public ICommand SaveProjectFileCommand { get; private set; }
    public ICommand ExitCommand { get; private set; }
    #endregion
    
    public IProjectsService? ProjectsSvc { get; }
    
}