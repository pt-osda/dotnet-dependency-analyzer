using System.IO;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.PackageUtils
{
    public class PackageLoader
    {
        /// <summary>
        /// Loads packages from packages.config file.
        /// </summary>
        /// <param name="path">Path to packages file.</param>
        /// <returns></returns>
        public static NuGetPackages LoadPackages(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return LoadPackages(stream);
            }
        }

        /// <summary>
        /// Loads packages from packages.config file.
        /// </summary>
        /// <param name="path">Stream of packages file.</param>
        /// <returns></returns>
        private static NuGetPackages LoadPackages(Stream packageConfig)
        {
            var serializer = new XmlSerializer(typeof(NuGetPackages));
            return (NuGetPackages)serializer.Deserialize(packageConfig);
        }
    }
}
