using FractalSurface3.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace FractalSurface3.ViewModels.Dialogs;

public class ReportDialogViewModel : ViewModelBase
{
    public ReportDialogViewModel()
    {
        SettingsSvc ??= App.Current?.Services?.GetService<ISettingsService>();
    }

    public ISettingsService? SettingsSvc { get; }
}