using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DotnetDependencyAnalyzer
{
    [Serializable]
    public class Report
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("name")]
        public object Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("build_tag")]
        public string BuildTag { get; set; }

        [JsonProperty("dependencies")]
        public List<Dependency> Dependencies { get; set; }

        public Report(string id, string version, string name, string description, string timestamp, string buildTag)
        {
            Id = id;
            Version = version;
            Name = name;
            Description = description;
            Timestamp = timestamp;
            BuildTag = buildTag;
        }
    }

    public class Dependency
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("main_version")]
        public string MainVersion { get; set; }

        [JsonProperty("vulnerabilities_count")]
        public int VulnerabilitiesCount { get; set; }

        [JsonProperty("licenses")]
        public List<License> Licenses { get; set; }

        [JsonProperty("vulnerabilities")]
        public List<Vulnerability> Vulnerabilities { get; set; }

        public Dependency(string title, string mainVersion, string description, int vulnerabilitiesCount)
        {
            Title = title;
            MainVersion = mainVersion;
            Description = description;
            VulnerabilitiesCount = vulnerabilitiesCount;
            Licenses = new List<License>();
        }
    }

    [Serializable]
    public class License
    {
        [JsonProperty("spdx_id")]
        public string Title { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        public License(string name, string source)
        {
            Title = name;
            Source = source;
        }
    }

    [Serializable]
    public class Vulnerability
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("references")]
        public List<string> References { get; set; }

        [JsonProperty("versions")]
        public List<string> Versions { get; set; }
    }
}
