using DotnetDependencyAnalyzer.NetCore.PackageUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotnetDependencyAnalyzer.NetCore.Licenses
{
    public class LicenseManager
    {
        private static readonly string proxyUrl = "http://35.234.147.77/nuget/dependency/{0}/{1}/licenses?licenseUrl={2}";

        /// <summary>
        /// Requests server proxy to get the license of a package.
        /// </summary>
        /// <param name="package">Package information.</param>
        /// <param name="maxAge">Specifies the maximum of time a resource in proxy cache is considered valid.</param>
        /// <returns></returns>
        public static async Task<List<License>> TryGetLicenseName(PackageInfo package, int maxAge)
        {
            List<License> licenses = new List<License>();
            if (!package.HasLicense())
            {
                return licenses;
            }

            string licenseUrl = package.LicenseUrl;
            if (IsLicenseUrl(licenseUrl, out string licenseName))
            {
                licenses.Add(new License(licenseName, $"Specified license URL {licenseUrl} in package specifications"));
                return licenses;
            }

            HttpClient httpClient = DependencyAnalyzer.Client;
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, string.Format(proxyUrl, package.Id, package.Version, licenseUrl));
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OSDA_PROXY_TOKEN"));
            req.Headers.CacheControl = new CacheControlHeaderValue() { MaxAge = new TimeSpan(0, 0, maxAge) };
            HttpResponseMessage resp;
            try
            {
                resp = await httpClient.SendAsync(req);
            }
            catch (HttpRequestException)
            {
                throw new Exception("Cannot reach central server.");
            }

            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
            string respBody = await resp.Content.ReadAsStringAsync();
            License[] licensesResp = JsonConvert.DeserializeObject<License[]>(respBody);
            licenses.AddRange(licensesResp);

            return licenses;
        }

        /// <summary>
        /// Checks if a given URL is related to a license.
        /// </summary>
        /// <param name="licenseUrl">A URL.</param>
        /// <param name="licenseName">Out parameter that is affected with the license SPDX identifier if the given URL is related to a license.</param>
        /// <returns> </returns>
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
