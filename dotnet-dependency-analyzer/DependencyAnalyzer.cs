using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotnetDependencyAnalyzer.NetCore.Licenses;
using DotnetDependencyAnalyzer.NetCore.PackageUtils;
using DotnetDependencyAnalyzer.NetCore.Vulnerabilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using System.Net.Http.Headers;

namespace DotnetDependencyAnalyzer.NetCore
{
    public class DependencyAnalyzer
    {
        private static readonly string projectFile = "{0}.csproj";
        private static readonly string nugetPropsFile = "{0}/obj/{1}.csproj.nuget.g.props";
        private static string projectPath;
        private static string packagesRootPath;
        private static string projectAssetsPath;

        private static List<string> licensesFetchErrors = new List<string>();
        private static bool vulnerabilitiesFetchError = false;

        private static readonly string reportAPIUrl = "http://35.234.147.77/report";

        public static HttpClient Client { get; } = new HttpClient();

        public static void Main(string[] args)
        {
            //CommandLineUtils.PrintLogo();
            projectPath = (args.Length == 0) ? "./" : args[0];
            if(Directory.GetFiles(projectPath, "*.csproj").Length == 0)
            {
                Console.WriteLine( args.Length==0 ? "Project not found in current directory" : "Project not found in the directory specified");
                return;
            }
            DateTime startTime = DateTime.UtcNow;
            CommandLineUtils.PrintInfoMessage("Plugin is running... ");
            string projectName = new DirectoryInfo(projectPath).Name;
            RetrieveNugetProperties(string.Format(nugetPropsFile, projectPath, projectName));
            CommandLineUtils.PrintInfoMessage($"Project Name: {projectName}");
            string projectFilePath = Path.Combine(projectPath, string.Format(projectFile, projectName));

            if (TryGetPolicy(out ProjectPolicy policy))
            {
                if (File.Exists(projectFilePath))
                {
                    CommandLineUtils.PrintInfoMessage("Finding project dependencies...  ");
                    List<NuGetPackage> packagesFound = PackageLoader.LoadPackages(projectFilePath, projectAssetsPath)
                                                    .Distinct()
                                                    .ToList();
                    CommandLineUtils.AppendSuccessMessage(Console.CursorLeft, Console.CursorTop, "DONE");
                    CommandLineUtils.PrintInfoMessage("Searching for dependencies licenses and vulnerabilities...  ");
                    int cursorLeft = Console.CursorLeft, cursorTop = Console.CursorTop;
                    List<Dependency> dependenciesEvaluated = ValidateProjectDependencies(packagesFound, policy).Result;
                    CommandLineUtils.AppendSuccessMessage(cursorLeft, cursorTop, "DONE");
                    string report = GenerateReport(dependenciesEvaluated, policy);
                    CommandLineUtils.PrintSuccessMessage("Produced report locally.");
                    StoreReport(report);
                    double seconds = (DateTime.UtcNow - startTime).TotalSeconds;
                    CommandLineUtils.PrintInfoMessage("Plugin execution time: " + seconds);
                }
                else
                {
                    CommandLineUtils.PrintErrorMessage($"Packages.config file not found in project {projectName}");
                }
            }
        }

        /// <summary>
        /// Retrieves path for packages folder and assets file
        /// </summary>
        /// <param name="propertiesFilePath">Path of NuGet proeprties file.</param>
        private static void RetrieveNugetProperties(string propertiesFilePath)
        {
            var serializer = new XmlSerializer(typeof(NuGetProperties));
            using (var stream = File.OpenRead(propertiesFilePath))
            {
                NuGetProperties props = (NuGetProperties)serializer.Deserialize(stream);
                packagesRootPath = props.PropertyGroup[0].NuGetPackageRoot.Value.Replace("$(UserProfile)", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                projectAssetsPath = props.PropertyGroup[0].ProjectAssetsFile.Value;
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
            if (policy.Admin == "")
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
            foreach (NuGetPackage package in packages)
            {
                PackageInfo packageInfo = PackageManager.GetPackageInfo(packagesRootPath, package.Id, package.Version);
                string packageDescription = (packageInfo == null) ? "" : packageInfo.Description;
                dependencies.Add(new Dependency(package.Id, package.Version, packageDescription, package.Direct, package.Children));
                dependenciesLicenses[i] = new List<License>();
                // It is not necessary to analyze System libraries license, only vulnerabilities 
                if (packageInfo != null && (packageInfo.ProjectUrl != "https://dot.net/" || !package.Id.StartsWith("System.")) )
                {
                    try
                    {
                        dependenciesLicenses[i] = await LicenseManager.TryGetLicenseName(packageInfo, policy.ApiCacheTime);
                    }
                    catch (Exception e)
                    {
                        CommandLineUtils.PrintWarningMessage($"An error occurred while trying to fetch {package.Id}.{package.Version} license: {e.Message}");
                        dependenciesLicenses[i] = new List<License>();
                        licensesFetchErrors.Add($"{package.Id}.{package.Version}");
                    }
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
            File.WriteAllText(Path.Combine(projectPath, "report.json"), jsonReport);
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
                var result = Client.SendAsync(req).Result;
                if (result.IsSuccessStatusCode)
                {
                    CommandLineUtils.PrintSuccessMessage("Report stored with success");
                }
                else
                {
                    CommandLineUtils.PrintErrorMessage("An error occurred during Report storage.");
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
