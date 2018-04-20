using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.PackageUtils
{
    public class PackageLoader
    {
        public static NuGetPackages LoadPackages(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return LoadPackages(stream);
            }
        }

        private static NuGetPackages LoadPackages(Stream packageConfig)
        {
            var serializer = new XmlSerializer(typeof(NuGetPackages));
            return (NuGetPackages)serializer.Deserialize(packageConfig);
        }
    }
}
