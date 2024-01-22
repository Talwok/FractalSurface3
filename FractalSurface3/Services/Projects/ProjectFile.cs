using System.IO;
using FractalSurface3.Helpers;
using FractalSurface3.Helpers.Configuration.Json;

namespace FractalSurface3.Services.Projects;

public class ProjectFile(Stream stream)
    : ZipJsonVersionedFile(stream, LastVersion, FileType)
{
    private const string FileType = "FractalSurfaceProjectFile";
    public static readonly SemVersion Version1_0_0 = new(1);
    public static readonly SemVersion LastVersion = Version1_0_0;
}