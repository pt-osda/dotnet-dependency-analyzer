
namespace DotnetDependencyAnalyzer.PackageUtils
{
    public class PackageInfo
    {
        public string LicenseUrl { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public string ProjectUrl { get; set; }
        public string Description { get; set; }

        public bool HasLicense()
        {
            return LicenseUrl != null && LicenseUrl != "";
        }
    }
}
