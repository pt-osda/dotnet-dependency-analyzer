using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    /// <summary>
    /// Represents Project element in .csproj file.
    /// </summary>
    [XmlRoot(ElementName = "Project")]
    public class Project
    {
        /// <summary>
        /// List of package groups.
        /// </summary>
        [XmlElement(ElementName = "ItemGroup")]
        public List<ItemGroup> ItemGroups { get; set; }
    }

    
    /// <summary>
    /// Represents ItemGroup element in .csproj file.
    /// </summary>
    [XmlRoot(ElementName = "ItemGroup")]
    public class ItemGroup
    {
        /// <summary>
        /// List of packages.
        /// </summary>
        [XmlElement(ElementName = "PackageReference")]
        public List<NuGetPackage> Packages { get; set; }
    }

    /// <summary>
    /// Represents PackageReference element in .csproj file.
    /// </summary>
    [XmlType(TypeName = "PackageReference")]
    public class NuGetPackage
    {
        /// <summary>
        /// Package id.
        /// </summary>
        [XmlAttribute(AttributeName = "Include")]
        public string Id { get; set; }

        /// <summary>
        /// Package version.
        /// </summary>
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// Dependency level (TRUE if direct, FALSE if transitive ).
        /// </summary>
        public bool Direct { get; set; }

        /// <summary>
        /// Child dependencies.
        /// </summary>
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