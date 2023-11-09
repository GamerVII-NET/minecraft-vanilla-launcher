
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MProjectInfo
{
    [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("project_type")]
        public string ProjectType { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("organization")]
        public object Organization { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("body_url")]
        public object BodyUrl { get; set; }

        [JsonProperty("published")]
        public DateTime Published { get; set; }

        [JsonProperty("updated")]
        public DateTime Updated { get; set; }

        [JsonProperty("approved")]
        public DateTime Approved { get; set; }

        [JsonProperty("queued")]
        public object Queued { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("requested_status")]
        public object RequestedStatus { get; set; }

        [JsonProperty("moderator_message")]
        public object ModeratorMessage { get; set; }

        [JsonProperty("client_side")]
        public string ClientSide { get; set; }

        [JsonProperty("server_side")]
        public string ServerSide { get; set; }

        [JsonProperty("downloads")]
        public int Downloads { get; set; }

        [JsonProperty("followers")]
        public int Followers { get; set; }

        [JsonProperty("categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("additional_categories")]
        public List<object> AdditionalCategories { get; set; }

        [JsonProperty("game_versions")]
        public List<string> GameVersions { get; set; }

        [JsonProperty("loaders")]
        public List<string> Loaders { get; set; }

        [JsonProperty("versions")]
        public List<string> Versions { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        [JsonProperty("issues_url")]
        public string IssuesUrl { get; set; }

        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }

        [JsonProperty("wiki_url")]
        public object WikiUrl { get; set; }

        [JsonProperty("discord_url")]
        public string DiscordUrl { get; set; }

        [JsonProperty("donation_urls")]
        public List<object> DonationUrls { get; set; }

        [JsonProperty("gallery")]
        public List<object> Gallery { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("monetization_status")]
        public string MonetizationStatus { get; set; }
}
