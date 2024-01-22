using System;
using System.IO;

namespace FractalSurface3.Helpers.Configuration.Json
{
    public class ZipJsonFileInfo : IEquatable<ZipJsonFileInfo>
    {
        public static ZipJsonFileInfo Empty { get; } = new();
        public string? FileVersion { get; set; } = string.Empty;
        public string? FileType { get; set; } = string.Empty;
        
        public bool Equals(ZipJsonFileInfo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FileVersion == other.FileVersion && FileType == other.FileType;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ZipJsonFileInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FileVersion != null ? FileVersion.GetHashCode() : 0) * 397) ^ (FileType != null ? FileType.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ZipJsonFileInfo? left, ZipJsonFileInfo? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ZipJsonFileInfo? left, ZipJsonFileInfo? right)
        {
            return !Equals(left, right);
        }
    }

    public interface IVersionedFile : IConfiguration
    {
        SemVersion FileVersion { get; }
    }

    public abstract class ZipJsonVersionedFile: ZipJsonConfiguration, IVersionedFile
    {
        private readonly SemVersion _version;
        private const string InfoKey = "FileInfo";
        protected ZipJsonVersionedFile(Stream zipStream, SemVersion lastVersion, string fileType) : base(zipStream)
        {
            var info = Get(InfoKey, ZipJsonFileInfo.Empty);
            string? type;
            if (info.Equals(ZipJsonFileInfo.Empty))
            {
                Set(InfoKey, new ZipJsonFileInfo
                {
                    FileVersion = lastVersion.ToString(),
                    FileType = fileType,
                });
                _version = lastVersion;
                type = fileType;
            }
            else
            {
                if (SemVersion.TryParse(info.FileVersion ?? string.Empty, out _version) == false)
                {
                    throw new Exception($"Can't read file version. (Want 'X.X.X', got '{info.FileVersion}')");
                }
                if (_version > lastVersion)
                {
                    throw new Exception($"Unsupported file version. (Want '{lastVersion}', got '{_version}')");
                }
                type = info.FileType;
            }
            if (string.Equals(type, fileType, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                throw new Exception($"Unsupported file type. (Want '{fileType}', got '{type}')");
            }
        }
    
        public SemVersion FileVersion => _version;
    }
}
