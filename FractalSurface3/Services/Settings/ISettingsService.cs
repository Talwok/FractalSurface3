using System;
using System.Reactive.Subjects;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Settings;

public interface ISettingsService : IDisposable
{
    public void Load();

    public void Save();

    public void Clear();

    public App.AppSettings AppSettings { get; }

    [Reactive]
    public bool IsChanged { get; set; }

}