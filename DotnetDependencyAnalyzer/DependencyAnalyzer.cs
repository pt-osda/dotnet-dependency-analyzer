using System;
using System.Collections.Generic;
using System.IO;
using DotnetDependencyAnalyzer.Licenses;
using DotnetDependencyAnalyzer.PackageUtils;
using DotnetDependencyAnalyzer.Vulnerabilities;

namespace DotnetDependencyAnalyzer
{
    public class DependencyAnalyzer
    {
        private static readonly string packagesFile = "packages.config";
        private static readonly string packagesPath = "../packages";
        private static readonly string projectPath = "./";

        public static void Main(string[] args)
        {
            Console.WriteLine("Plugin is running... ");
            DirectoryInfo projectDir = new DirectoryInfo(projectPath);
            Console.WriteLine($"Project Name: {projectDir.Name}");
            string nugetFile = Path.Combine(projectDir.FullName, packagesFile);
            if (File.Exists(nugetFile))
            {
                NuGetPackages packages = PackageLoader.LoadPackages(nugetFile);
                ValidateProjectDependencies(packages);
            }
            else
            {
                Console.WriteLine($"Packages.config file not found in project {projectDir.Name}");
            }
        }

        private static bool ValidateProjectDependencies(List<NuGetPackage> packages)
        {
            foreach (NuGetPackage package in packages)
            {
                Console.WriteLine();
                String packageDir = $"{package.Id}.{package.Version}";
                PackageInfo packageInfo = PackageManager.GetPackageInfo(Path.Combine(projectPath, packagesPath, packageDir));
                string licenseName;
                if (!packageInfo.HasLicense())
                {
                    licenseName = "No license";
                }
                else if(!LicenseManager.TryGetLicenseName(packageInfo, out licenseName))
                {
                    licenseName = "Cannot find license name";
                }
                Console.WriteLine($"Version: {package.Version}, Id: {package.Id}, License: {licenseName}");
                VulnerabilityEvaluationResult result = VulnerabilityEvaluation.EvaluatePackage(packageInfo.Id, packageInfo.Version);
                Console.WriteLine($"{result.VulnerabilitiesNumber} Vulnerabilities Found");
                if (result.VulnerabilitiesNumber > 0)
                {
                    int i = 1;
                    foreach (Vulnerability v in result.VulnerabilitiesFound)
                    {
                        Console.WriteLine($"{i++}: {v.Title} - {v.Description}");
                    }
                }
            }
            return true;
        }
    }
}
