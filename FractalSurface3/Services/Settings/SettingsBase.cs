using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData.Binding;
using FractalSurface3.Helpers;
using FractalSurface3.Helpers.Async;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Settings;

public abstract class SettingsBase : DisposableReactiveObjectWithValidation
{
    private readonly List<SettingsBase> _children = new();

    [Reactive]
    public bool IsChanged { get; set; }

    public void Load(SettingsFile file)
    {
        InternalLoad(file);
        _children.ForEach(x => x.Load(file));
        IsChanged = false;
    }

    protected abstract void InternalLoad(SettingsFile file);

    public void Save(SettingsFile file)
    {
        InternalSave(file);
        _children.ForEach(x => x.Save(file));
        IsChanged = false;
    }
    
    protected abstract void InternalSave(SettingsFile file);

    protected void InternalAddChild(SettingsBase child)
    {
        child.WhenValueChanged(x => x.IsChanged)
            .Where(x => x)
            .Subscribe(_ => IsChanged = true)
            .DisposeItWith(Disposable);
        child.DisposeItWith(Disposable);
        _children.Add(child);
    }
    
    protected void InternalThrowUnknownFileVersion(string name, SemVersion wantVersion, SemVersion gotVersion)
    {
        throw new Exception($"Save '{name}' error: unknown file version. Want '{wantVersion}'. Got '{gotVersion}'");
    }
    
    protected void InternalAddParam(string sideName)
    {
        this.WhenAnyPropertyChanged(sideName)
            .Subscribe(_ => IsChanged = true).DisposeItWith(Disposable);
    }
}

public abstract class SettingsBase<TParent>(TParent parent) : SettingsBase
    where TParent : SettingsBase
{
    public TParent Parent { get; } = parent;
}