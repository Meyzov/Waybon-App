using SQLite;

namespace Waybon.App.Data.Entities
{
    public class LocalGroupMember
    {
        [Indexed]
        public int GroupId { get; set; }

        [Indexed]
        public Guid UserId { get; set; }
    }
}