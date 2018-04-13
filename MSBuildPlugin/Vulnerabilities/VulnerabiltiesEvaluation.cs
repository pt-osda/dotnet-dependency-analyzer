using Newtonsoft.Json;
using System.Net.Http;

namespace DotnetDependencyAnalyzer.Vulnerabilities
{
    public class VulnerabiltiesEvaluation
    {
        private static readonly string apiUriTemplate = "https://ossindex.net/v2.0/package/nuget/{0}/{1}";
        private static readonly HttpClient client = new HttpClient();

        public static VulnerabilityEvaluationResult EvaluatePackage(string packageId, string version)
        {
            string requestUri = string.Format(apiUriTemplate, packageId, version);
            string resp = client.GetStringAsync(requestUri).Result;
            VulnerabilityEvaluationResult[] result = JsonConvert.DeserializeObject<VulnerabilityEvaluationResult[]>(resp);
            return result[0];
        }
    }
}
