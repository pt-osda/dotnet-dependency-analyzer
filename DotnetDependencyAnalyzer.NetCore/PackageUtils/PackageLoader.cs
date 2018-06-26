using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    public class PackageLoader
    {
        public static List<NuGetPackage> LoadPackages(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return LoadPackages(stream);
            }
        }

        private static List<NuGetPackage> LoadPackages(Stream packageConfig)
        {
            var serializer = new XmlSerializer(typeof(Project));
            Project prj = (Project)serializer.Deserialize(packageConfig);
            return prj.ItemGroup.Packages;
        }
    }
}
