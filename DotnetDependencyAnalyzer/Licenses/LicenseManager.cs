using DotnetDependencyAnalyzer.PackageUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetDependencyAnalyzer.Licenses
{
    public class LicenseManager
    {
        private static readonly string proxyUrl = "http://localhost:8080/nuget/dependency/{0}/{1}/licenses?licenseUrl={2}";

        public static async Task<List<License>> TryGetLicenseName(PackageInfo package)
        {
            List<License> licenses = new List<License>();
            string licenseUrl = package.LicenseUrl;

            if (IsLicenseUrl(licenseUrl, out string licenseName))
            {
                licenses.Add(new License(licenseName, $"Specified license URL {licenseUrl} in package specifications"));
                return licenses;
            }

            HttpClient httpClient = DependencyAnalyzer.Client;
            HttpResponseMessage resp = await httpClient.GetAsync(string.Format(proxyUrl, package.Id, package.Version,licenseUrl));
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
    }
}
