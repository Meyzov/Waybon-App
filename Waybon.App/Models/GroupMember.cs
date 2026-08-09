using System.Text.Json.Serialization;

namespace Waybon.App.Models
{
    public class GroupMember
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool SharingEnabled { get; set; }
        public bool BlockedByMe { get; set; }
        public bool BlockingMe { get; set; }
        public DateTime? LastActivityAt { get; set; }

        [JsonIgnore]
        public string DisplayUsername { get; set; } = string.Empty;

        [JsonIgnore]
        public string LastActivityText { get; set; } = string.Empty;

        [JsonIgnore]
        public List<string> Tags { get; set; } = [];
    }
}