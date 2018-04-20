using NuGet.Packaging;
using System.IO;

namespace DotnetDependencyAnalyzer.PackageUtils
{
    public class PackageManager
    {
        public static PackageInfo GetPackageInfo(string path)
        {
            string packageFilePath = GetPackageFilePath(path);
            NuspecReader packageReader = new PackageArchiveReader(packageFilePath).NuspecReader;
            return new PackageInfo
            {
                Id = packageReader.GetId(),
                Version = packageReader.GetVersion().OriginalVersion,
                LicenseUrl = packageReader.GetLicenseUrl(),
                ProjectUrl = packageReader.GetProjectUrl()
            };
        }

        private static string GetPackageFilePath(string path)
        {
            string packageFile = new DirectoryInfo(path).Name + ".nupkg";
            return Path.Combine(path, packageFile);
        }
    }
}
