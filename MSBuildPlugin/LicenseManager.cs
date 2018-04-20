using DotnetDependencyAnalyzerMSBuildTask;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotnetDependencyAnalyzerMSBuildTask
{
    public class LicenseManager
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string githubUrl = "https://github.com/";
        private static readonly string githubApiUrl = "https://developer.github.com/v3/repos/";

        public static string GetLicenseName(PackageInfo package)
        {
            string licenseUrl = package.LicenseUrl;
            HttpContent respContent;
            if (licenseUrl.StartsWith(githubUrl))
            {
                licenseUrl = getPlainTextUrl(licenseUrl);
            }
            respContent = client.GetAsync(licenseUrl).Result.Content;
            if (respContent.Headers.ContentType.MediaType == "text/plain")
            {
                StreamReader respStream = new StreamReader(respContent.ReadAsStreamAsync().Result);
                string firstLine;
                while ((firstLine = respStream.ReadLine()) == "");
                return firstLine.TrimStart(' ').TrimEnd(' ');
            }
            string projectUrl = package.ProjectUrl;
            if (projectUrl.StartsWith(githubUrl))
            {
                string[] path = projectUrl.Replace(githubUrl, "").Split('/');
                string owner = path[0];
                string repo = path[1];
                //respContent = 
            }
            return "Cannot Find License Name";
        }

        private static string getPlainTextUrl(string githubUrl)
        {
            return githubUrl
                .Replace("github", "raw.githubusercontent")
                .Replace("/blob", "");
        }
    }
}