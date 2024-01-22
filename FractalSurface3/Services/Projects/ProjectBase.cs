using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData.Binding;
using FractalSurface3.Helpers;
using FractalSurface3.Helpers.Async;
using FractalSurface3.Services.Settings;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Projects;

public abstract class ProjectBase : DisposableReactiveObjectWithValidation
{
    private readonly List<ProjectBase> _children = new();

    [Reactive]
    public bool IsChanged { get; set; }

    public void Load(ProjectFile file)
    {
        InternalLoad(file);
        _children.ForEach(x => x.Load(file));
        IsChanged = false;
    }

    protected abstract void InternalLoad(ProjectFile file);

    public void Save(ProjectFile file)
    {
        InternalSave(file);
        _children.ForEach(x => x.Save(file));
        IsChanged = false;
    }
    
    protected abstract void InternalSave(ProjectFile file);

    protected void InternalAddChild(ProjectBase child)
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

public abstract class ProjectBase<TParent>(TParent parent) : ProjectBase
    where TParent : ProjectBase
{
    public TParent Parent { get; } = parent;
}