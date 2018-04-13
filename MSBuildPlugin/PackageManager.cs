using NuGet.Packaging;
using System.IO;

namespace DotnetDependencyAnalyzer
{
    public class PackageManager
    {
        public static PackageInfo GetPackageInfo(string hintPath)
        {
            string packageFilePath = GetPackageFilePath(hintPath);
            NuspecReader packageReader = new PackageArchiveReader(packageFilePath).NuspecReader;
            return new PackageInfo
            {
                Id = packageReader.GetId(),
                Version = packageReader.GetVersion().OriginalVersion,
                LicenseUrl = packageReader.GetLicenseUrl()
            };
        }

        private static string GetPackageFilePath(string hintPath)
        {
            string packageDir = Path.GetFullPath(Path.Combine(hintPath, @"..\..\..\"));
            string packageFile = new DirectoryInfo(packageDir).Name + ".nupkg";
            return Path.Combine(packageDir, packageFile);
        }
    }
}
