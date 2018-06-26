using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "Project")]
    public class NuGetProperties
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "PropertyGroup")]
        public List<PropertyGroup> PropertyGroup { get; set; }
    }

    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "PropertyGroup")]
    public class PropertyGroup
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003",  ElementName = "NuGetPackageRoot")]
        public NuGetPackageRoot NuGetPackageRoot { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "ProjectAssetsFile")]
        public ProjectAssetsFile ProjectAssetsFile { get; set; }
    }

    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "ProjectAssetsFile")]
    public class ProjectAssetsFile
    {
        [XmlText]
        public string Value { get; set; }
    }

    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "NuGetPackageRoot")]
    public class NuGetPackageRoot
    {
        [XmlText]
        public string Value { get; set; }
    }
}
