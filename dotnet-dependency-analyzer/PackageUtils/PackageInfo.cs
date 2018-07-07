
namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    /// <summary>
    /// Package information obtained from .nuspec file.
    /// </summary>
    public class PackageInfo
    {
        /// <summary>
        /// License URL of the package.
        /// </summary>
        public string LicenseUrl { get; set; }

        /// <summary>
        /// Package id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Package version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Package URL.
        /// </summary>
        public string ProjectUrl { get; set; }

        /// <summary>
        /// Package description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Checks if a project has a license.
        /// </summary>
        /// <returns></returns>
        public bool HasLicense()
        {
            return LicenseUrl != null && LicenseUrl != "";
        }
    }
}
