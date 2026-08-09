using SQLite;

namespace Waybon.App.Data.Entities
{
    public class LocalGroup
    {
        [PrimaryKey]
        public int GroupId { get; set; }

        public Guid OwnerUserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? JoinCode { get; set; }

        public DateTime? JoinCodeExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}