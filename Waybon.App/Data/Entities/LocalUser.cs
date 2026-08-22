using SQLite;

namespace Waybon.App.Data.Entities
{
    public class LocalUser
    {
        [PrimaryKey]
        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public bool SharingEnabled { get; set; }

        public bool BlockedByMe { get; set; }

        public bool BlockingMe { get; set; }

        public DateTime? LastActivityAt { get; set; }
    }
}