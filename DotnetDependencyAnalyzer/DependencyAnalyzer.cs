using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using DotnetDependencyAnalyzer.Licenses;
using DotnetDependencyAnalyzer.PackageUtils;
using DotnetDependencyAnalyzer.Vulnerabilities;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotnetDependencyAnalyzer
{
    public class DependencyAnalyzer
    {
        private static readonly string packagesFile = "packages.config";
        private static readonly string packagesPath = "../packages";
        private static readonly string projectPath = "./";

        private static readonly string pluginId = "DotnetDependencyAnalyzer";

        public static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Console.WriteLine("Plugin is running... ");
            DirectoryInfo projectDir = new DirectoryInfo(projectPath);
            Console.WriteLine($"Project Name: {projectDir.Name}");
            string nugetFile = Path.Combine(projectDir.FullName, packagesFile);
            if (File.Exists(nugetFile))
            {
                List<NuGetPackage> packagesFound = PackageLoader.LoadPackages(nugetFile)
                                                .Where(package => package.Id != pluginId)
                                                .ToList();
                List<Dependency> dependenciesEvaluated = ValidateProjectDependencies(packagesFound);
                GenerateReport(dependenciesEvaluated);
                Console.WriteLine("Produced report.");
            }
            else
            {
                Console.WriteLine($"Packages.config file not found in project {projectDir.Name}");
            }
            Console.ReadLine();
        }

        private static List<Dependency> ValidateProjectDependencies(List<NuGetPackage> packages)
        {
            Task<Dependency>[] packageEvaluationTasks = new Task<Dependency>[packages.Count];
            int i = 0;
            foreach (NuGetPackage package in packages)
            {
                packageEvaluationTasks[i++] = Task.Factory.StartNew(() => ValidateDependency(package));
            }
            Task.WaitAll(packageEvaluationTasks);
            return packageEvaluationTasks.Select(task => task.Result).ToList();
        }

        private static Dependency ValidateDependency(NuGetPackage package)
        {
            String packageDir = $"{package.Id}.{package.Version}";
            PackageInfo packageInfo = PackageManager.GetPackageInfo(Path.Combine(projectPath, packagesPath, packageDir));

            Dependency dependency = new Dependency(package.Id, package.Version)
            {
                License = LicenseManager.TryGetLicenseName(packageInfo)
            };

            VulnerabilityEvaluationResult vulnerabilityEvaluation = VulnerabilityEvaluation.EvaluatePackage(packageInfo.Id, packageInfo.Version);
            if(vulnerabilityEvaluation.VulnerabilitiesFound != null)
            {
                dependency.Vulnerabilities = vulnerabilityEvaluation.VulnerabilitiesFound;
            }

            return dependency;
        }

        private static void GenerateReport(List<Dependency> dependenciesEvaluated)
        {
            Report report = new Report("Id", "1.0.0", "Report", "Report")
            {
                Dependencies = dependenciesEvaluated
            };
            string jsonReport = JsonConvert.SerializeObject(report);
            File.WriteAllText(Path.Combine(projectPath,"report.json"), jsonReport);
        }
    }
}
