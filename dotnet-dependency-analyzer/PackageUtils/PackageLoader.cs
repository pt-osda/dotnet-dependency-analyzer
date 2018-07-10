using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DotnetDependencyAnalyzer.NetCore.PackageUtils
{
    public class PackageLoader
    {
        private static readonly string assetsDependency = "{0}/{1}";

        /// <summary>
        /// Loads direct dependencies from .csproj file and indirect dependencies from assets.json file.
        /// </summary>
        /// <param name="path">Path to .csproj file.</param>
        /// <param name="assetsFilePath">Path to assets.json file.</param>
        /// <returns></returns>
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
                    .Select(package =>
                    {
                        if (package.Version.StartsWith("$(")) {
                            package.Version = GetPackageVersionFromAssets(targets[framework], package.Id);
                        }
                        package.Direct = true;
                        string dependencyEntry = string.Format(assetsDependency, package.Id, package.Version);
                        package.Children = GetChildren(
                            targets[framework][dependencyEntry]
                        );
                        return package;
                    })
                    .Where(package => package.Version != null)
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

        private static string GetPackageVersionFromAssets(JToken jToken, string id)
        {
            foreach(var props in jToken.OfType<JProperty>())
            {
                string [] dependency = props.Name.Split("/");
                if(dependency[0] == id)
                {
                    return dependency[1];
                }
            }
            return null;
        }

        /// <summary>
        /// Loads packages from cs.proj file.
        /// </summary>
        /// <param name="path">Stream of packages file.</param>
        /// <returns></returns>
        private static List<NuGetPackage> LoadPackages(Stream packageConfig)
        {
            var serializer = new XmlSerializer(typeof(Project));
            Project prj = (Project)serializer.Deserialize(packageConfig);
            return prj.ItemGroups
                .SelectMany(it => it.Packages)
                .Where(package => package.Id != null && IsValidVersion(package))
                .ToList();
        }

        private static bool IsValidVersion(NuGetPackage package)
        {
            return package.Version.StartsWith("$(") || Version.TryParse(package.Version, out Version v);
        }

        /// <summary>
        /// Retrieves all child dependencies of a given dependency.
        /// </summary>
        /// <param name="dependency">JToken object that represents a dependency in assets file.</param>
        /// <returns></returns>
        private static List<string> GetChildren(JToken dependency)
        {
            List<string> deps = new List<string>();
            if (dependency != null && dependency["dependencies"] != null)
            {
                foreach (JProperty prop in dependency["dependencies"])
                {
					deps.Add($"{prop.Name}:{prop.Value}");
                }
            }
            return deps;
        }

        /// <summary>
        /// Gets recursively every indirect dependencies related to a direct dependency.
        /// </summary>
        /// <param name="children">Child dependencies of a direct dependency.</param>
        /// <param name="dependencies">JToken object that represents every dependencies in assets file.</param>
        /// <returns></returns>
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
                if (IsValidVersion(package))
                {
                    deps.Add(package);
                    deps.AddRange(GetIndirectDependencies(package.Children, dependencies));
                }
            }
            return deps;
        }
    }
}
