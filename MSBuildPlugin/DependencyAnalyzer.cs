using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using System.Threading.Tasks;
using System.IO;
using NuGet.Packaging;
using System.Net.Http;
using System;
using Newtonsoft.Json;

namespace MSBuildPlugin
{
    public class DependencyAnalyzer : Microsoft.Build.Utilities.Task
    {
        public string ProjectFilePath { get; set; }
        private readonly string _id = "DependencyAnalyzer";
        private readonly string apiUriTemplate = "https://ossindex.net/v2.0/package/nuget/{0}/{1}";

        public override bool Execute()
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Plugin is running... ", "", _id, MessageImportance.High));
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Project File: " + ProjectFilePath, "", _id, MessageImportance.High));
            return ValidateProjectDependencies(new Project(ProjectFilePath)).Result;
        }

        private async Task<bool> ValidateProjectDependencies(Project project)
        {
            HttpClient client = new HttpClient();
            foreach (ProjectItem item in project.AllEvaluatedItems)
            {
                if (item.HasMetadata("HintPath"))
                {
                    string hintPath = item.GetMetadataValue("HintPath");
                    string packageDir = Path.GetFullPath(Path.Combine(hintPath, @"..\..\..\"));
                    string packageFile = new DirectoryInfo(packageDir).Name + ".nupkg";
                    string packageFilePath = Path.Combine(packageDir, packageFile);
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs("\nPackage found: " + packageFilePath, "", _id, MessageImportance.High));
                    NuspecReader packageReader = new PackageArchiveReader(packageFilePath).NuspecReader;
                    string licenseUrl = packageReader.GetLicenseUrl();
                    string version = packageReader.GetVersion().OriginalVersion;
                    string id = packageReader.GetId();
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("Version: {0}; Id: {1}; License: {2}", version, id, licenseUrl), "", _id, MessageImportance.High));
                    string requestUri = string.Format(apiUriTemplate,id,version);
                    string resp = await client.GetStringAsync(requestUri);
                    VulnerabilityEvaluation[] result = JsonConvert.DeserializeObject<VulnerabilityEvaluation[]>(resp);
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs(result[0].VulnerabilitiesNumber + " Vulnerabilities Found", "", _id, MessageImportance.High));
                    if (result[0].VulnerabilitiesNumber > 0)
                    {
                        int i = 1;
                        foreach (Vulnerability v in result[0].VulnerabilitiesFound)
                        {
                            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(i + ") " + v.Title + " : " + v.Description, "", _id, MessageImportance.High));
                            ++i;
                        }
                    }
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs("", "", _id, MessageImportance.High));
                }
            }
            return true;
        }
    }

    [Serializable]
    public class VulnerabilityEvaluation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vulnerability-matches")]
        public int VulnerabilitiesNumber { get; set; }

        [JsonProperty("vulnerabilities")]
        public Vulnerability[] VulnerabilitiesFound { get; set; }
    }

    [Serializable]
    public class Vulnerability
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("versions")]
        public string[] Versions { get; set; }
    }
}
