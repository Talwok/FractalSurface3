using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FractalSurface3.Helpers;
using FractalSurface3.Helpers.Async;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Settings;

public class SettingsService : DisposableReactiveObject, ISettingsService
{ 
    private const string SettingsFileName = "Settings.fsset";
    
    public SettingsService()
    {
        AppSettings = new App.AppSettings();

        AppSettings.WhenAnyValue(_ => _.IsChanged)
            .Subscribe(isChanged => IsChanged = isChanged)
            .DisposeItWith(Disposable);
    }

    public void Load()
    {
        using var fileStream = new FileStream(SettingsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.ReadWrite);

        using var settingsFile = new SettingsFile(fileStream);
        
        AppSettings.Load(settingsFile);
    }

    public void Save()
    {
        using var fileStream = new FileStream(SettingsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.ReadWrite);

        using var settingsFile = new SettingsFile(fileStream);
        
        AppSettings.Save(settingsFile);
    }

    public void Clear()
    {
        File.Delete(SettingsFileName);
        Load();
        Save();
    }
    
    [Reactive]
    public bool IsChanged { get; set; }
    
    public App.AppSettings AppSettings { get; }
}