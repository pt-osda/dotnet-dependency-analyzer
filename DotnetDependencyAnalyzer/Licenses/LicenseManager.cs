using DotnetDependencyAnalyzer.PackageUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotnetDependencyAnalyzer.Licenses
{
    public class LicenseManager
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string githubUrl = "https://github.com/";
        private static readonly string githubApiUrl = "https://api.github.com/repos";

        public static List<License> TryGetLicenseName(PackageInfo package)
        {
            List<License> licenses = new List<License>();
            string licenseUrl = package.LicenseUrl;

            if (IsLicenseUrl(licenseUrl, out string licenseName))
            {
                licenses.Add(new License(licenseName, "Specified license URL in package specifications"));
            }

            if (licenseUrl.StartsWith(githubUrl))
            {
                licenseUrl = getPlainTextUrl(licenseUrl);
            }
            HttpResponseMessage resp = client.GetAsync(licenseUrl).Result;
            if (resp.IsSuccessStatusCode)
            {
                string licenseContent = resp.Content.ReadAsStringAsync().Result;
                if (FindLicenseNameInFile(licenseContent, out licenseName) || FindLicenseUrlInFile(licenseContent, out licenseName))
                {
                    string source = $"Found license name or url in {licenseUrl}";
                    License license = licenses.Find(l => licenseName == l.Title);
                    if(license == null)
                    {
                        licenses.Add(new License(licenseName, source));
                    }
                    else
                    {
                        license.AddSource(source);
                    }
                }
            }

            string projectUrl = package.ProjectUrl;
            if (IsGithubProject(projectUrl) && FindLicenseInGithubRepo(projectUrl, out licenseName))
            {
                string source = $"Specified license in Github project: {projectUrl}";
                License license = licenses.Find(l => licenseName == l.Title);
                if (license == null)
                {
                    licenses.Add(new License(licenseName, source));
                }
                else
                {
                    license.AddSource(source);
                }
            }

            return licenses;
        }

        private static bool IsLicenseUrl(string licenseUrl, out string licenseName)
        {
            licenseName = null;
            if (KnownLicenses.licensesUrl.ContainsKey(licenseUrl))
            {
                licenseName = KnownLicenses.licensesUrl[licenseUrl];
                return true;
            }
            return false;
        }

        private static string getPlainTextUrl(string githubUrl)
        {
            return githubUrl
                .Replace("github", "raw.githubusercontent")
                .Replace("/blob", "");
        }

        private static bool FindLicenseNameInFile(string licenseContent, out string licenseName)
        {
            licenseName = null;
            foreach (string name in KnownLicenses.licensesName)
            {
                if (licenseContent.Contains(name))
                {
                    licenseName = name;
                    return true;
                }
            }
            return false;
        }

        private static bool FindLicenseUrlInFile(string licenseContent, out string licenseName)
        {
            licenseName = null;
            foreach (string url in KnownLicenses.licensesUrl.Keys)
            {
                if (licenseContent.Contains(url))
                {
                    licenseName = KnownLicenses.licensesUrl[url];
                    return true;
                }
            }
            return false;
        }

        private static bool IsGithubProject(string projectUrl)
        {
            return projectUrl.StartsWith(githubUrl);
        }

        private static bool FindLicenseInGithubRepo(string projectUrl, out string licenseName)
        {
            licenseName = null;
            string[] path = projectUrl.Replace(githubUrl, "").Split('/');
            string owner = path[0];
            string repo = path[1];
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            HttpResponseMessage resp = client.GetAsync($"{githubApiUrl}/{owner}/{repo}/license").Result;
            if (!resp.IsSuccessStatusCode)
            {
                return false;
            }
            string json = resp.Content.ReadAsStringAsync().Result;
            JObject licenseObj = JObject.Parse(json);
            licenseName = (string)licenseObj["license"]["name"];
            if(licenseName != "Other")
            {
                return true;
            }
            string licenseEncoding = (string) licenseObj["encoding"];
            string encodedLicenseContent = (string)licenseObj["content"];
            string decodedLicenseContent = Encoding.UTF8.GetString(Convert.FromBase64String(encodedLicenseContent));
            return FindLicenseNameInFile(decodedLicenseContent, out licenseName) || FindLicenseUrlInFile(decodedLicenseContent, out licenseName);
        }
    }
}
