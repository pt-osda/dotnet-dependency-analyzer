using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    public class PackageLoader
    {
        private static readonly string assetsDependency = "{0}/{1}";

        public static List<NuGetPackage> LoadPackages(string path, string assetsFilePath)
        {
            using (var stream = File.OpenRead(path))
            {
                string assets = File.ReadAllText(assetsFilePath);
                JObject dependencySearch = JObject.Parse(assets);
                JToken targets = dependencySearch["targets"];
                string framework = targets.Children().First().Path.Replace("targets['", "").Replace("']", "");

                // get direct dependencies
                List<NuGetPackage> packages = LoadPackages(stream)
                    .Where(package => package.Id != "DotnetDependencyAnalyzer.NetCore")
                    .ToList();
                packages = packages
                    .Select(package =>
                    {
                        package.Direct = true;
                        string dependencyEntry = string.Format(assetsDependency, package.Id, package.Version);
                        package.Children = GetChildren(
                            targets[framework][dependencyEntry]
                        );
                        return package;
                    })
                    .ToList();

                // get indirect dependencies
                List<NuGetPackage> indirectDependencies = new List<NuGetPackage>();
                foreach(NuGetPackage package in packages)
                {
                    string dependencyEntry = string.Format(assetsDependency, package.Id, package.Version);
                    indirectDependencies.AddRange(GetIndirectDependencies(package.Children, targets[framework]));
                }
                packages.AddRange(indirectDependencies);

                return packages;
            }
        }

        private static List<NuGetPackage> LoadPackages(Stream packageConfig)
        {
            var serializer = new XmlSerializer(typeof(Project));
            Project prj = (Project)serializer.Deserialize(packageConfig);
            return prj.ItemGroup.Packages;
        }

        private static List<string> GetChildren(JToken dependency)
        {
            List<string> deps = new List<string>();
            if (dependency != null && dependency["dependencies"] != null)
            {
                foreach (JProperty prop in dependency["dependencies"])
                {
                    if (!prop.Name.StartsWith("System."))
                    {
                        deps.Add($"{prop.Name}:{prop.Value}");
                    }
                }
            }
            return deps;
        }

        private static List<NuGetPackage> GetIndirectDependencies(List<string> children, JToken dependencies)
        {
            List<NuGetPackage> deps = new List<NuGetPackage>();
            foreach(string child in children)
            {
                NuGetPackage package = new NuGetPackage
                {
                    Id = child.Split(":")[0],
                    Version = child.Split(":")[1],
                    Direct = false,
                    Children = GetChildren(dependencies[child.Replace(":", "/")])
                };
                deps.Add(package);
                deps.AddRange(GetIndirectDependencies(package.Children, dependencies));
            }
            return deps;
        }
    }
}
