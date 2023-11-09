using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MProject
{
    [JsonProperty("slug")] public string Slug { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("categories")] public List<string> Categories { get; set; }

    [JsonProperty("client_side")] public string ClientSide { get; set; }

    [JsonProperty("server_side")] public string ServerSide { get; set; }

    [JsonProperty("body")] public string Body { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("requested_status")] public string RequestedStatus { get; set; }

    [JsonProperty("additional_categories")]
    public List<string> AdditionalCategories { get; set; }

    [JsonProperty("issues_url")] public string IssuesUrl { get; set; }

    [JsonProperty("source_url")] public string SourceUrl { get; set; }

    [JsonProperty("wiki_url")] public string WikiUrl { get; set; }

    [JsonProperty("discord_url")] public string DiscordUrl { get; set; }

    [JsonProperty("donation_urls")] public List<MDonationUrl> DonationUrls { get; set; }

    [JsonProperty("project_type")] public string ProjectType { get; set; }

    [JsonProperty("downloads")] public int Downloads { get; set; }

    [JsonProperty("icon_url")] public string IconUrl { get; set; }

    [JsonProperty("color")] public int Color { get; set; }

    [JsonProperty("thread_id")] public string ThreadId { get; set; }

    [JsonProperty("monetization_status")] public string MonetizationStatus { get; set; }

    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("team")] public string Team { get; set; }

    [JsonProperty("body_url")] public object BodyUrl { get; set; }

    [JsonProperty("moderator_message")] public object ModeratorMessage { get; set; }

    [JsonProperty("published")] public string Published { get; set; }

    [JsonProperty("updated")] public string Updated { get; set; }

    [JsonProperty("approved")] public string Approved { get; set; }

    [JsonProperty("queued")] public string Queued { get; set; }

    [JsonProperty("followers")] public int Followers { get; set; }

    [JsonProperty("license")] public MLicense MLicense { get; set; }

    [JsonProperty("versions")] public List<string> Versions { get; set; }

    [JsonProperty("game_versions")] public List<string> GameVersions { get; set; }

    [JsonProperty("loaders")] public List<string> Loaders { get; set; }

    [JsonProperty("gallery")] public List<MGallery> Gallery { get; set; }
}
