using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using NuGet.Packaging;

namespace MSBuildPlugin
{
    public class DependencyAnalyzer : Task
    {
        public string ProjectFilePath { get; set; }
        private const string _id = "DependencyAnalyzer";

        public override bool Execute()
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Plugin is running... ", "", _id, MessageImportance.High));
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Project File: " + ProjectFilePath, "", _id, MessageImportance.High));
            return ValidateProjectDependencies(new Project(ProjectFilePath));
        }

        private bool ValidateProjectDependencies(Project project)
        {
            foreach (ProjectItem item in project.AllEvaluatedItems)
            {
                if (item.HasMetadata("HintPath"))
                {
                    string hintPath = item.GetMetadataValue("HintPath");
                    string packageDir = Path.GetFullPath(Path.Combine(hintPath, @"..\..\..\"));
                    string packageFile = new DirectoryInfo(packageDir).Name + ".nupkg";
                    string packageFilePath = Path.Combine(packageDir, packageFile);
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Package found: " + packageFilePath, "", _id, MessageImportance.High));
                    NuspecReader packageReader = new PackageArchiveReader(packageFilePath).NuspecReader;
                    string licenseUrl = packageReader.GetLicenseUrl();
                    string version = packageReader.GetVersion().OriginalVersion;
                    string id = packageReader.GetId();
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("Version: {0}; Id: {1}; License: {2}", version, id, licenseUrl), "", _id, MessageImportance.High));
                }
            }
            return true;
        }
    }
}
