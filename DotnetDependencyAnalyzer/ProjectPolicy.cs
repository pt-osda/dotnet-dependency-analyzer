using Newtonsoft.Json;
using System;

namespace DotnetDependencyAnalyzer
{
    [Serializable]
    public class ProjectPolicy
    {
        [JsonProperty("project_id")]
        public string Id { get; set; }

        [JsonProperty("project_name")]
        public string Name { get; set; }

        [JsonProperty("project_version")]
        public string Version { get; set; }

        [JsonProperty("project_description")]
        public string Description { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("repo")]
        public string Repo { get; set; }

        [JsonProperty("repo_owner")]
        public string RepoOwner { get; set; }

        [JsonProperty("invalid_licenses")]
        public string [] InvalidLicenses { get; set; }

        [JsonProperty("plugin_cache_time")]
        public int PluginCacheTime { get; set; }

        [JsonProperty("api_cache_time")]
        public int ApiCacheTime { get; set; }
    }
}
