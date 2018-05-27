using DotnetDependencyAnalyzer.PackageUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace DotnetDependencyAnalyzer.Licenses
{
    public class LicenseManager
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string proxyUrl = "http://localhost:8080/nuget/dependency/{0}/{1}/licenses?licenseUrl={2}";

        public static List<License> TryGetLicenseName(PackageInfo package)
        {
            List<License> licenses = new List<License>();
            string licenseUrl = package.LicenseUrl;

            if (IsLicenseUrl(licenseUrl, out string licenseName))
            {
                licenses.Add(new License(licenseName, $"Specified license URL {licenseUrl} in package specifications"));
                return licenses;
            }

            /*if (licenseUrl.StartsWith(githubUrl))
            {
                licenseUrl = getPlainTextUrl(licenseUrl);
            }*/
            HttpResponseMessage resp = client.GetAsync(string.Format(proxyUrl, package.Id, package.Version,licenseUrl)).Result;
            if (resp.IsSuccessStatusCode)
            {
                string content = resp.Content.ReadAsStringAsync().Result;
                License[] licensesResp = JsonConvert.DeserializeObject<License[]>(content);
                licenses.AddRange(licensesResp);
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
    }
}
