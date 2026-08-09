using SQLite;

namespace Waybon.App.Data.Entities
{
    public class UserLocation
    {
        [PrimaryKey]
        public Guid UserId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime LocationUpdatedAt { get; set; }
    }
}