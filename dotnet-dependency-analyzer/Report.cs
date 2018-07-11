using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DotnetDependencyAnalyzer.NetCore
{
    /// <summary>
    /// Represents the report model.
    /// </summary>
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

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("repo")]
        public string Repo { get; set; }

        [JsonProperty("repo_owner")]
        public string RepoOwner { get; set; }

        [JsonProperty("admin")]
        public string Admin { get; set; }

        [JsonProperty("error_info")]
        public string ErrorInfo { get; set; }

        [JsonProperty("dependencies")]
        public List<Dependency> Dependencies { get; set; }

        public Report(string id, string version, string name, string description, string timestamp, string organization, string repo, string repoOwner, string admin, string errorInfo)
        {
            Id = id;
            Version = version;
            Name = name;
            Description = description;
            Timestamp = timestamp;
            Organization = organization;
            Repo = repo;
            RepoOwner = repoOwner;
            Admin = admin;
            ErrorInfo = errorInfo;
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

        [JsonProperty("direct")]
        public bool Direct { get; set; }

        [JsonProperty("children")]
        public List<string> Children { get; set; }

        public Dependency(string title, string mainVersion, string description, bool direct, List<string> children)
        {
            Title = title;
            MainVersion = mainVersion;
            Description = description;
            Licenses = new List<License>();
            Vulnerabilities = new List<Vulnerability>();
            Direct = direct;
            Children = children;
        }
    }

    [Serializable]
    public class License
    {
        [JsonProperty("spdx_id")]
        public string Title { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }

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
