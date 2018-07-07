using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.PackageUtils
{
    /// <summary>
    /// List of NuGet packages extracted from packages.config file.
    /// </summary>
    [XmlRoot(Namespace = "", ElementName = "packages")]
    public class NuGetPackages : List<NuGetPackage>
    {
    }

    /// <summary>
    /// NuGet package extracted from packages.config file.
    /// </summary>
    [XmlType(TypeName = "package")]
    public class NuGetPackage
    {
        /// <summary>
        /// Package id.
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Package version.
        /// </summary>
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}