using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    [XmlRoot(ElementName = "Project")]
    public class Project
    {
        [XmlElement(ElementName = "ItemGroup")]
        public ItemGroup ItemGroup { get; set; }
    }

    [XmlRoot(ElementName = "ItemGroup")]
    public class ItemGroup
    {
        [XmlElement(ElementName = "PackageReference")]
        public List<NuGetPackage> Packages { get; set; }
    }

    [XmlType(TypeName = "PackageReference")]
    public class NuGetPackage
    {
        [XmlAttribute(AttributeName = "Include")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        public bool Direct { get; set; }

        public List<string> Children { get; set; } = new List<string>();

        public override bool Equals(object obj)
        {
            NuGetPackage package = (NuGetPackage) obj;
            return package.Id == Id && package.Version == Version;
        }

        public override int GetHashCode()
        {
            return new { Id, Version }.GetHashCode();
        }
    }
}