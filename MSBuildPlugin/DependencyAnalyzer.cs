using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
            Project project = new Project(ProjectFilePath);
            foreach (ProjectItem item in project.AllEvaluatedItems)
            {
                if (item.HasMetadata("HintPath"))
                {
                    string value = item.GetMetadataValue("HintPath");
                    BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Package found: " + value, "", _id, MessageImportance.High));
                }
            }
            return true;
        }
    }
}
