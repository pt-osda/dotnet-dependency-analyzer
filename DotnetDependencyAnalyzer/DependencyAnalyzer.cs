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
using System.Net.Http;
using System.Text;

namespace DotnetDependencyAnalyzer
{
    public class DependencyAnalyzer
    {
        private static readonly string packagesFile = "packages.config";
        private static readonly string packagesPath = "../packages";
        private static string projectPath;

        private static readonly string pluginId = "DotnetDependencyAnalyzer";
        
        private static readonly string reportAPIUrl = "http://localhost:8080/report";
        private static readonly HttpClient client = new HttpClient();

        public static void Main(string[] args)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Console.WriteLine("Plugin is running... ");
            projectPath = "./" + args[0];
            DirectoryInfo projectDir = new DirectoryInfo(projectPath);
            Console.WriteLine($"Project Name: {projectDir.Name}");
            string nugetFile = Path.Combine(projectDir.FullName, packagesFile);

            if (File.Exists(nugetFile))
            {
                List<NuGetPackage> packagesFound = PackageLoader.LoadPackages(nugetFile)
                                                .Where(package => package.Id != pluginId)
                                                .ToList();
                List<Dependency> dependenciesEvaluated = ValidateProjectDependencies(packagesFound);
                string report = GenerateReport(dependenciesEvaluated, projectDir.Name);
                Console.WriteLine("Produced report locally.");
                StoreReport(report);
            }
            else
            {
                Console.WriteLine($"Packages.config file not found in project {projectDir.Name}");
            }
        }

        private static List<Dependency> ValidateProjectDependencies(List<NuGetPackage> packages)
        {
            List<Dependency> dependencies = new List<Dependency>();
            VulnerabilityEvaluationResult[] vulnerabilityEvaluation = VulnerabilityEvaluation.EvaluatePackage(packages);

            Task<List<License>>[] findLicenseTasks = new Task<List<License>>[packages.Count];
            int i = 0;
            foreach (NuGetPackage package in packages)
            {
                String packageDir = $"{package.Id}.{package.Version}";
                PackageInfo packageInfo = PackageManager.GetPackageInfo(Path.Combine(projectPath, packagesPath, packageDir));
                dependencies.Add(new Dependency(packageInfo.Id, packageInfo.Version, packageInfo.Description, vulnerabilityEvaluation[i].VulnerabilitiesNumber)
                {
                    Vulnerabilities = vulnerabilityEvaluation[i].VulnerabilitiesFound ?? new List<Vulnerability>()
                });
                findLicenseTasks[i++] = Task.Factory.StartNew(() => LicenseManager.TryGetLicenseName(packageInfo));
            }

            Task.WaitAll(findLicenseTasks);
            i = 0;
            foreach (Task<List<License>> task in findLicenseTasks)
            {
                dependencies[i++].Licenses = task.Result;
            }

            return dependencies;
        }

        private static string GenerateReport(List<Dependency> dependenciesEvaluated, string projectName)
        {
            Report report = new Report("Id", "1.0.0", projectName, "Report", DateTime.Now.ToString("yyyyMMddHHmmssffff"), "Build Tag")
            {
                Dependencies = dependenciesEvaluated
            };
            string jsonReport = JsonConvert.SerializeObject(report);
            File.WriteAllText(Path.Combine(projectPath,"report.json"), jsonReport);
            return jsonReport;
        }

        private static void StoreReport(string report)
        {
            using (var client = new HttpClient())
            {
                var result = client.PostAsync(reportAPIUrl, new StringContent(report, Encoding.UTF8, "application/json")).Result;
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Report stored with success");
                }
                else
                {
                    Console.WriteLine("An error during Report storage");
                }
            }
        }
    }
}
