using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore
{
    /// <summary>
    /// NuGet properties extracted from nuget.g.props file.
    /// </summary>
    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "Project")]
    public class NuGetProperties
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "PropertyGroup")]
        public List<PropertyGroup> PropertyGroup { get; set; }
    }
    
    /// <summary>
    /// Group of NuGet properties.
    /// </summary>
    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "PropertyGroup")]
    public class PropertyGroup
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003",  ElementName = "NuGetPackageRoot")]
        public NuGetPackageRoot NuGetPackageRoot { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "ProjectAssetsFile")]
        public ProjectAssetsFile ProjectAssetsFile { get; set; }
    }

    /// <summary>
    /// Property that contains in its value the project assets file path.
    /// </summary>
    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "ProjectAssetsFile")]
    public class ProjectAssetsFile
    {
        [XmlText]
        public string Value { get; set; }
    }

    /// <summary>
    /// Property that contains in its value the NuGet packages root path.
    /// </summary>
    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", ElementName = "NuGetPackageRoot")]
    public class NuGetPackageRoot
    {
        [XmlText]
        public string Value { get; set; }
    }
}
