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

        [JsonProperty("dependencies")]
        public List<Dependency> Dependencies { get; set; }

        public Report(string id, string version, string name, string description)
        {
            Id = id;
            Version = version;
            Name = name;
            Description = description;
        }
    }

    public class Dependency
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("main_version")]
        public string MainVersion { get; set; }

        /*[JsonProperty("private_versions")]
        public List<string> PrivateVersions { get; set; }*/

        [JsonProperty("license")]
        public List<License> License { get; set; }

        /*[JsonProperty("hierarchy")]
        public List<Dependency> Hierarchy { get; set; }*/

        [JsonProperty("vulnerabilities")]
        public List<Vulnerability> Vulnerabilities { get; set; }

        public Dependency(string title, string mainVersion)
        {
            Title = title;
            MainVersion = mainVersion;
            License = new List<License>();
            Vulnerabilities = new List<Vulnerability>();
        }
    }

    [Serializable]
    public class License
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("origins")]
        public List<string> Sources { get; set; }

        public License(string name, string source)
        {
            Title = name;
            Sources = new List<string>{source};
        }

        public void AddSource(string source)
        {
            Sources.Add(source);
        }
    }

    [Serializable]
    public class Vulnerability
    {
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
