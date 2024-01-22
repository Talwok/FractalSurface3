using FractalSurface3.Helpers.Configuration;
using FractalSurface3.Services.Settings.App.Report;

namespace FractalSurface3.Services.Settings.App;

public class AppSettings : SettingsBase
{
    public AppSettings()
    {
        InternalAddChild(ReportSettings = new ReportSettings(this));
    }
    
    protected override void InternalLoad(SettingsFile file)
    {
        // here we can load from file only params of this class. Children data will be already loaded.
        if (file.FileVersion <= SettingsFile.Version1_0_0)
        {
            LoadV1(file);
        }
        else
        {
            InternalThrowUnknownFileVersion(nameof(AppSettings),SettingsFile.LastVersion,file.FileVersion);
        }
    }

    protected override void InternalSave(SettingsFile file)
    {
        // here we can save to file only params of this class. Children will be already saved.
        if (file.FileVersion <= SettingsFile.Version1_0_0)
        {
            SaveV1(file);    
        }
        else
        {
            InternalThrowUnknownFileVersion(nameof(AppSettings),SettingsFile.Version1_0_0,file.FileVersion);
        }        
    }

    
    private void LoadV1(SettingsFile file)
    {
        var config = file.Get<AppSettingsModelV1>();
    }

    private void SaveV1(SettingsFile file)
    {
        file.Set(new AppSettingsModelV1
        {

        });
    }
    
    public ReportSettings ReportSettings { get; }
}