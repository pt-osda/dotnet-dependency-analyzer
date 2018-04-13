using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using DotnetDependencyAnalyzer.Vulnerabilities;
using DotnetDependencyAnalyzer;

namespace MSBuildPlugin
{
    public class DependencyAnalyzer : Microsoft.Build.Utilities.Task
    {
        public string ProjectFilePath { get; set; }
        private readonly string _id = "DependencyAnalyzer";

        public override bool Execute()
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Plugin is running... ", "", _id, MessageImportance.High));
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Project File: " + ProjectFilePath, "", _id, MessageImportance.High));
            Project project = new Project(ProjectFilePath);
            return ValidateProjectDependencies(project);
        }

        private bool ValidateProjectDependencies(Project project)
        {
            foreach (ProjectItem item in project.AllEvaluatedItems)
            {
                if (item.HasMetadata("HintPath"))
                {
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs("", "", _id, MessageImportance.High));
                    string hintPath = item.GetMetadataValue("HintPath");
                    PackageInfo package = PackageManager.GetPackageInfo(hintPath);
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("Version: {0}; Id: {1}; License: {2}", package.Version, package.Id, package.LicenseUrl), "", _id, MessageImportance.High));
                    VulnerabilityEvaluationResult result = VulnerabiltiesEvaluation.EvaluatePackage(package.Id, package.Version);
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs(result.VulnerabilitiesNumber + " Vulnerabilities Found", "", _id, MessageImportance.High));
                    if (result.VulnerabilitiesNumber > 0)
                    {
                        int i = 1;
                        foreach (Vulnerability v in result.VulnerabilitiesFound)
                        {
                            BuildEngine.LogMessageEvent(new BuildMessageEventArgs( (i++) + ") " + v.Title + " : " + v.Description, "", _id, MessageImportance.High));
                        }
                    }
                }
            }
            return true;
        }
    }
}
