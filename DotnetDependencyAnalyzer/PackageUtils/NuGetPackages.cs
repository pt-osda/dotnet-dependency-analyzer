using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.PackageUtils
{
    [XmlRoot(Namespace = "", ElementName = "packages")]
    public class NuGetPackages : List<NuGetPackage>
    {
    }

    [XmlType(TypeName = "package")]
    public class NuGetPackage
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}