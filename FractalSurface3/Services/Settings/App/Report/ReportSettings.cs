using System;
using System.IO;
using FractalSurface3.Helpers.Configuration;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Settings.App.Report;

public class ReportSettings : SettingsBase<AppSettings>
{
    public ReportSettings(AppSettings parent) : base(parent)
    {
        InternalAddParam(nameof(OutputDirectory));
        InternalAddParam(nameof(TemplatesDirectory));
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
            InternalThrowUnknownFileVersion(nameof(ReportSettings),SettingsFile.LastVersion,file.FileVersion);
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
            InternalThrowUnknownFileVersion(nameof(ReportSettings),SettingsFile.Version1_0_0,file.FileVersion);
        }        
    }
    
    private void LoadV1(SettingsFile file)
    {
        var config = file.Get(new ReportSettingsModelV1
        {
            OutputDirectory = Path.Combine(Environment.CurrentDirectory, "Reports", "Output"),
            TemplatesDirectory = Path.Combine(Environment.CurrentDirectory, "Reports", "Templates")
        });
        
        OutputDirectory = config.OutputDirectory;
        TemplatesDirectory = config.TemplatesDirectory;
    }

    private void SaveV1(SettingsFile file)
    {
        file.Set(new ReportSettingsModelV1
        {
            OutputDirectory = OutputDirectory,
            TemplatesDirectory = TemplatesDirectory
        });
    }

    [Reactive] 
    public string OutputDirectory { get; set; } = string.Empty;
    
    [Reactive]
    public string TemplatesDirectory { get; set; } = string.Empty;
}