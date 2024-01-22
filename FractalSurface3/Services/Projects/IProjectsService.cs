using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Projects;

public interface IProjectsService
{
    public Task CreateProjectFileAsync();
    public Task OpenProjectFileAsync();
    public Task SaveProjectFileAsAsync();
    
    [Reactive]
    bool IsChanged { get; set; }
}