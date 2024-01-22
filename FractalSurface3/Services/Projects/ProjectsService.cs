using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using DynamicData.Binding;
using FractalSurface3.Helpers;
using FractalSurface3.Helpers.Async;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Projects;

public class ProjectsService : DisposableReactiveObject, IProjectsService
{
    private const string NewFileName = "NewProject";
    
    private readonly IStorageProvider? _storageProvider;
    private readonly FilePickerFileType _projectFileType;

    public ProjectsService()
    {
        _storageProvider ??= App.Current?.Services?.GetService<IStorageProvider>();

        _projectFileType = new FilePickerFileType("FractalSurface Project")
        {
            Patterns = new[] { "*.fsproj" },
            MimeTypes = new[] { "application/fsproj" },
            AppleUniformTypeIdentifiers = new[] { "zip.fsproj" }
        };
        
        CurrentProject = new Project();

        CurrentProject.WhenValueChanged(x => x.IsChanged)
            .Subscribe(isChanged => IsChanged = isChanged)
            .DisposeItWith(Disposable);

    }

    public async Task CreateProjectFileAsync()
    {
        var file = await _storageProvider?.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Создание файла проекта",
            SuggestedFileName = NewFileName,
            DefaultExtension = _projectFileType.Name,
            FileTypeChoices = new []
            {
                _projectFileType
            },
            ShowOverwritePrompt = true
        })!;

        if(file == null) return;
        
        await using var stream = new FileStream(file.Path.LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.ReadWrite);

        using var projectFile = new ProjectFile(stream);
        
        CurrentProject.Load(projectFile);
        CurrentProject.Save(projectFile);
    }
    
    public async Task OpenProjectFileAsync()
    {
        var files = await _storageProvider?.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выбор файла проекта",
            AllowMultiple = false,
            FileTypeFilter = new [] { _projectFileType }
        })!;

        if(files.Count == 0) return;
        
        var file = files.First();

        await using var stream = new FileStream(file.Path.LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.ReadWrite);
        
        using var projectFile = new ProjectFile(stream);
        
        CurrentProject.Load(projectFile);
    } 
    
    public async Task SaveProjectFileAsAsync()
    {
        var file = await _storageProvider?.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранение файла проекта",
            SuggestedFileName = NewFileName,
            DefaultExtension = _projectFileType.Name,
            FileTypeChoices = new []
            {
                _projectFileType
            },
            ShowOverwritePrompt = true
        })!;

        if(file == null) return;
                
        await using var stream = new FileStream(file.Path.LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.ReadWrite);

        using var projectFile = new ProjectFile(stream);
        
        CurrentProject.Save(projectFile);
    }
    
    
    public async Task OpenImagesAsync()
    {
        var images = await _storageProvider?.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выбор файлов изображений",
            AllowMultiple = true,
            FileTypeFilter = new [] { FilePickerFileTypes.ImageAll }
        })!;
        
        if(images.Count == 0) return;
        
        
    }
    
    public Project CurrentProject { get; }
    
    [Reactive]
    public bool IsChanged { get; set; }
}