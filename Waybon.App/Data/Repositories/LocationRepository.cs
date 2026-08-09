using SQLite;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;

namespace Waybon.App.Data.Repositories
{
    public class LocationRepository(IDatabaseService database) : ILocationRepository
    {
        private readonly SQLiteAsyncConnection _db = database.Connection;

        public Task SaveLocationAsync(UserLocation location)
        {
            return _db.InsertOrReplaceAsync(location);
        }

        public Task<UserLocation> GetLocationAsync(Guid userId)
        {
            return _db.Table<UserLocation>().Where(l => l.UserId == userId).FirstOrDefaultAsync();
        }

        public Task<List<UserLocation>> GetLocationsAsync(IEnumerable<Guid> userIds)
        {
            var ids = userIds.ToList();
            if (ids.Count == 0)
            {
                return Task.FromResult(new List<UserLocation>());
            }

            return _db.Table<UserLocation>().Where(l => ids.Contains(l.UserId)).ToListAsync();
        }

        public Task ClearAllLocationsAsync()
        {
            return _db.DeleteAllAsync<UserLocation>();
        }
    }
}