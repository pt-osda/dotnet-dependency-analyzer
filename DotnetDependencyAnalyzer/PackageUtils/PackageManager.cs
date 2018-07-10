using NuGet.Packaging;
using System.IO;

namespace DotnetDependencyAnalyzer.PackageUtils
{
    public class PackageManager
    {
        /// <summary>
        /// Gets package specifications, after reading .nuspec file.
        /// </summary>
        /// <param name="path">Directory path of the package.</param>
        /// <returns></returns>
        public static PackageInfo GetPackageInfo(string path)
        {
            string packageFilePath = GetPackageFilePath(path);
            try
            {
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves nupkg file path.
        /// </summary>
        /// <param name="path">Directory path of the package.</param>
        /// <returns></returns>
        private static string GetPackageFilePath(string path)
        {
            string packageFile = new DirectoryInfo(path).Name + ".nupkg";
            return Path.Combine(path, packageFile);
        }
    }
}
