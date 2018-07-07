using NuGet.Packaging;
using System.IO;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    public class PackageManager
    {
        /// <summary>
        /// Gets package specifications, after reading .nuspec file.
        /// </summary>
        /// <param name="rootPath">Root path of all packages.</param>
        /// <param name="packageId">Package id.</param>
        /// <param name="packageVersion">Package version.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves nupkg file path.
        /// </summary>
        /// <param name="rootPath">Root path of all packages.</param>
        /// <param name="packageId">Package id.</param>
        /// <param name="packageVersion">Package version.</param>
        /// <returns></returns>
        private static string GetPackageFilePath(string rootPath, string packageId, string packageVersion)
        {
            string packageFile = $"{packageId.ToLower()}.{packageVersion}.nupkg";
            return Path.Combine(rootPath, packageId, packageVersion, packageFile);
        }
    }
}
