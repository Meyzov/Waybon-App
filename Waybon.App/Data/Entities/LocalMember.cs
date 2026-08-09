using SQLite;

namespace Waybon.App.Data.Entities
{
    public class LocalMember
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int GroupId { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public bool SharingEnabled { get; set; }

        public bool BlockedByMe { get; set; }

        public bool BlockingMe { get; set; }

        public DateTime? LastActivityAt { get; set; }
    }
}