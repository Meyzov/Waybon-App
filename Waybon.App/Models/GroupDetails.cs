using System.Text.Json.Serialization;

namespace Waybon.App.Models
{
    public class GroupDetails
    {
        public int GroupId { get; set; }
        public Guid OwnerUserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? JoinCode { get; set; }
        public DateTime? JoinCodeExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public string DisplayUsername { get; set; } = string.Empty;
    }
}