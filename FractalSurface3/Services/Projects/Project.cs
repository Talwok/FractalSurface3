using System.Collections.Generic;
using FractalSurface3.Helpers.Configuration;
using ReactiveUI.Fody.Helpers;

namespace FractalSurface3.Services.Projects;

public class Project : ProjectBase
{
    public Project()
    {
        InternalAddParam(nameof(Images));
    }

    protected override void InternalLoad(ProjectFile file)
    {
        // here we can load from file only params of this class. Children data will be already loaded.
        if (file.FileVersion <= ProjectFile.Version1_0_0)
        {
            LoadV1(file);
        }
        else
        {
            InternalThrowUnknownFileVersion(nameof(Project),ProjectFile.LastVersion,file.FileVersion);
        }
    }

    protected override void InternalSave(ProjectFile file)
    {
        // here we can save to file only params of this class. Children will be already saved.
        if (file.FileVersion <= ProjectFile.Version1_0_0)
        {
            SaveV1(file);    
        }
        else
        {
            InternalThrowUnknownFileVersion(nameof(Project),ProjectFile.Version1_0_0,file.FileVersion);
        }        
    }
    
    private void LoadV1(ProjectFile file)
    {
        var config = file.Get(new ProjectModelV1
        {
            Images = new Dictionary<string, byte[]>(),
        });
        Images = config.Images;
    }

    private void SaveV1(ProjectFile file)
    {
        file.Set(new ProjectModelV1
        {
            Images = Images,
        });
    }

    [Reactive] 
    public Dictionary<string, byte[]> Images { get; set; } = new ();
}