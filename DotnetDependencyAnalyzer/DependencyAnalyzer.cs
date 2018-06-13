using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly string reportAPIUrl = /*"http://35.234.147.77/report"*/    "http://localhost:8080/report";

        public static HttpClient Client { get; } = new HttpClient();

        public static void Main(string[] args)
        {
            DateTime startTime = DateTime.UtcNow;
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
                List<Dependency> dependenciesEvaluated = ValidateProjectDependencies(packagesFound).Result;
                string report = GenerateReport(dependenciesEvaluated, projectDir.Name);
                Console.WriteLine("Produced report locally.");
                StoreReport(report);
            }
            else
            {
                Console.WriteLine($"Packages.config file not found in project {projectDir.Name}");
            }
            double seconds = (DateTime.UtcNow - startTime).TotalSeconds;
            Console.WriteLine("Plugin execution time: " + seconds);
        }

        private static async Task<List<Dependency>> ValidateProjectDependencies(List<NuGetPackage> packages)
        {
            List<Dependency> dependencies = new List<Dependency>();

            VulnerabilityEvaluationResult[] vulnerabilityEvaluationResult = await VulnerabilityEvaluation.EvaluatePackage(packages);

            List<License>[] dependenciesLicenses = new List<License>[packages.Count];
            int i = 0;
            foreach(NuGetPackage package in packages)
            {
                String packageDir = $"{package.Id}.{package.Version}";
                PackageInfo packageInfo = PackageManager.GetPackageInfo(Path.Combine(projectPath, packagesPath, packageDir));
                dependencies.Add(new Dependency(packageInfo.Id, packageInfo.Version, packageInfo.Description));
                try
                {
                    dependenciesLicenses[i] = await LicenseManager.TryGetLicenseName(packageInfo);
                }
                catch (Exception) {
                    dependenciesLicenses[i] = new List<License>();
                }
                ++i;
            }

            i = 0;
            foreach (List<License> licenses in dependenciesLicenses)
            {
                dependencies[i].Licenses = licenses;
                dependencies[i].VulnerabilitiesCount = vulnerabilityEvaluationResult[i].VulnerabilitiesNumber;
                dependencies[i].Vulnerabilities = vulnerabilityEvaluationResult[i].VulnerabilitiesFound;
                ++i;
            }

            return dependencies;
        }

        private static string GenerateReport(List<Dependency> dependenciesEvaluated, string projectName)
        {
            string dateTime = string.Concat(DateTime.UtcNow.ToString("s"), "Z");
            Report report = new Report("Id", "1.0.0", projectName, "Report", dateTime, "Build Tag")
            {
                Dependencies = dependenciesEvaluated
            };
            string jsonReport = JsonConvert.SerializeObject(report);
            File.WriteAllText(Path.Combine(projectPath,"report.json"), jsonReport);
            return jsonReport;
        }

        private static void StoreReport(string report)
        {
            var result = Client.PostAsync(reportAPIUrl, new StringContent(report, Encoding.UTF8, "application/json")).Result;
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
