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
using System.Net.Http.Headers;

namespace DotnetDependencyAnalyzer
{
    public class DependencyAnalyzer
    {
        private static readonly string packagesFile = "packages.config";
        private static readonly string packagesPath = "../packages";
        private static string projectPath;
        private static readonly string pluginId = "DotnetDependencyAnalyzer";

        private static List<string> licensesFetchErrors = new List<string>();
        private static bool vulnerabilitiesFetchError = false;

        private static readonly string reportAPIUrl = "http://35.234.151.254/report";

        public static HttpClient Client { get; } = new HttpClient();

        public static void Main(string[] args)
        {
            DateTime startTime = DateTime.UtcNow;
            CommandLineUtils.PrintInfoMessage("Plugin is running... ");
            projectPath = args[0];
            DirectoryInfo projectDir = new DirectoryInfo(projectPath);
            CommandLineUtils.PrintInfoMessage($"Project Name: {projectDir.Name}");
            string nugetFile = Path.Combine(projectPath, packagesFile);

            if (TryGetPolicy(out ProjectPolicy policy))
            {
                if (File.Exists(nugetFile))
                {
                    CommandLineUtils.PrintInfoMessage("Finding project dependencies...  ");
                    List<NuGetPackage> packagesFound = PackageLoader.LoadPackages(nugetFile)
                                                    .Where(package => package.Id != pluginId)
                                                    .ToList();
                    CommandLineUtils.PrintSuccessMessage("Finding project dependencies DONE");
                    CommandLineUtils.PrintInfoMessage("Searching for dependencies licenses and vulnerabilities...  ");
                    List<Dependency> dependenciesEvaluated = ValidateProjectDependencies(packagesFound, policy).Result;
                    CommandLineUtils.PrintSuccessMessage("Searching for dependencies licenses and vulnerabilities DONE");
                    string report = GenerateReport(dependenciesEvaluated, policy);
                    CommandLineUtils.PrintSuccessMessage("Produced report locally.");
                    StoreReport(report);
                    double seconds = (DateTime.UtcNow - startTime).TotalSeconds;
                    CommandLineUtils.PrintInfoMessage("Plugin execution time: " + seconds);
                }
                else
                {
                    CommandLineUtils.PrintErrorMessage($"Packages.config file not found in project {projectDir.Name}");
                }
            }
        }

        /// <summary>
        /// Tries to get information about the project policy.
        /// </summary>
        /// <param name="policy">Object that represents a project policy.</param>
        /// <returns></returns>
        private static bool TryGetPolicy(out ProjectPolicy policy)
        {
            string[] osdaFiles = Directory.GetFiles(projectPath, ".osda");
            if (osdaFiles.Length == 0)
            {
                CommandLineUtils.PrintErrorMessage("This project does not contain a policy file (.osda).");
                policy = null;
                return false;
            }
            policy = JsonConvert.DeserializeObject<ProjectPolicy>(File.ReadAllText(osdaFiles[0]));
            if (policy.Id == "")
            {
                CommandLineUtils.PrintErrorMessage("Project id not specified in policy.");
                return false;
            }
            if (policy.Name == "")
            {
                CommandLineUtils.PrintErrorMessage("Project name not specified in policy.");
                return false;
            }
            if(policy.Admin == "")
            {
                CommandLineUtils.PrintErrorMessage("Project administrator name not specified in policy.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates project dependencies according to their vulnerabilities and licenses.
        /// </summary>
        /// <param name="packages">List of packages of the project.</param>
        /// <param name="policy">Project policy.</param>
        /// <returns></returns>
        private static async Task<List<Dependency>> ValidateProjectDependencies(List<NuGetPackage> packages, ProjectPolicy policy)
        {
            List<Dependency> dependencies = new List<Dependency>();
            VulnerabilityEvaluationResult[] vulnerabilityEvaluationResult = null;

            try
            {
                vulnerabilityEvaluationResult = await VulnerabilityEvaluation.EvaluatePackage(packages, policy.ApiCacheTime);
            }
            catch (Exception e)
            {
                CommandLineUtils.PrintWarningMessage($"An error occurred while trying to fetch dependencies vulnerabilities: {e.Message}");
                vulnerabilitiesFetchError = true;
            }

            List<License>[] dependenciesLicenses = new List<License>[packages.Count];
            int i = 0;
            foreach(NuGetPackage package in packages)
            {
                String packageDir = $"{package.Id}.{package.Version}";
                PackageInfo packageInfo = PackageManager.GetPackageInfo(Path.Combine(projectPath, packagesPath, packageDir));
                dependencies.Add(new Dependency(packageInfo.Id, packageInfo.Version, packageInfo.Description));
                try
                {
                    dependenciesLicenses[i] = await LicenseManager.TryGetLicenseName(packageInfo, policy.ApiCacheTime);
                }
                catch (Exception e) {
                    CommandLineUtils.PrintWarningMessage($"An error occurred while trying to fetch {package.Id}.{package.Version} license: {e.Message}");
                    dependenciesLicenses[i] = new List<License>();
                    licensesFetchErrors.Add($"{package.Id}.{package.Version}");
                }
                ++i;
            }

            i = 0;
            foreach (List<License> licenses in dependenciesLicenses)
            {
                dependencies[i].Licenses = licenses.Select(license =>
                    {
                        license.Valid = !policy.InvalidLicenses.Contains(license.Title);
                        return license;
                    })
                    .ToList();
                if (!vulnerabilitiesFetchError)
                {
                    dependencies[i].VulnerabilitiesCount = vulnerabilityEvaluationResult[i].VulnerabilitiesNumber;
                    dependencies[i].Vulnerabilities = vulnerabilityEvaluationResult[i].VulnerabilitiesFound;
                }
                ++i;
            }

            return dependencies;
        }

        /// <summary>
        /// Creates a report and stores it locally.
        /// </summary>
        /// <param name="dependenciesEvaluated">List of dependencies with its evaluation.</param>
        /// <param name="policy">Project policy.</param>
        /// <returns></returns>
        private static string GenerateReport(List<Dependency> dependenciesEvaluated, ProjectPolicy policy)
        {
            string dateTime = string.Concat(DateTime.UtcNow.ToString("s"), "Z");
            string errorInfo = GetErrorInfo();
            Report report = new Report(policy.Id, policy.Version, policy.Name, policy.Description, dateTime, policy.Organization, policy.Repo, policy.RepoOwner, policy.Admin, errorInfo)
            {
                Dependencies = dependenciesEvaluated
            };
            string jsonReport = JsonConvert.SerializeObject(report);
            File.WriteAllText(Path.Combine(projectPath,"report.json"), jsonReport);
            return jsonReport;
        }

        /// <summary>
        /// Stores a report on Central Server.
        /// </summary>
        /// <param name="report">Report to be stored.</param>
        private static void StoreReport(string report)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, reportAPIUrl)
            {
                Content = new StringContent(report, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("CENTRAL_SERVER_TOKEN"));
            try
            {
                var resp = Client.SendAsync(req).Result;
                if (resp.IsSuccessStatusCode)
                {
                    CommandLineUtils.PrintSuccessMessage("Report stored with success");
                }
                else
                {
                    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        CommandLineUtils.PrintErrorMessage("Invalid Token (CENTRAL_SERVER_TOKEN)");
                    }
                    else
                    {
                        CommandLineUtils.PrintErrorMessage("An error occurred during Report storage.");
                    }
                }
            }
            catch (HttpRequestException)
            {
                CommandLineUtils.PrintErrorMessage("An error occurred during Report storage.");
            }
        }

        /// <summary>
        /// Gets the information about errors that occurred during a report.
        /// </summary>
        /// <returns></returns>
        private static string GetErrorInfo()
        {
            string vulnerabilitiesErrorMessage = "An error occurred while trying to fetch dependencies vulnerabilities.";
            if (licensesFetchErrors.Count == 0)
            {
                return (vulnerabilitiesFetchError) ? vulnerabilitiesErrorMessage : null;
            }
            string licensesErrorMessage = $"An error occurred while trying to fetch licenses from the following dependencies: {string.Join(",", licensesFetchErrors)}. ";
            return (vulnerabilitiesFetchError) ? licensesErrorMessage + vulnerabilitiesErrorMessage : licensesErrorMessage;
        }
    }
}
