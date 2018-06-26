using NuGet.Packaging;
using System.IO;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    public class PackageManager
    {
        public static PackageInfo GetPackageInfo(string rootPath, string packageId, string packageVersion)
        {
            string packageFilePath = GetPackageFilePath(rootPath, packageId, packageVersion);
            NuspecReader packageReader = new PackageArchiveReader(packageFilePath).NuspecReader;
            return new PackageInfo
            {
                Id = packageReader.GetId(),
                Version = packageReader.GetVersion().OriginalVersion,
                LicenseUrl = packageReader.GetLicenseUrl(),
                ProjectUrl = packageReader.GetProjectUrl(),
                Description = packageReader.GetDescription()
            };
        }

        private static string GetPackageFilePath(string path, string packageId, string packageVersion)
        {
            string packageFile = $"{packageId.ToLower()}.{packageVersion}.nupkg";
            return Path.Combine(path, packageId, packageVersion, packageFile);
        }
    }
}
