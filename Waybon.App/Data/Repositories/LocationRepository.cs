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

        public async Task<UserLocation?> GetLocationAsync(Guid userId)
        {
            return await _db.Table<UserLocation>().Where(l => l.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserLocation>> GetLocationsAsync(IEnumerable<Guid> userIds)
        {
            var ids = userIds.ToList();
            if (ids.Count == 0)
            {
                return [];
            }

            return await _db.Table<UserLocation>().Where(l => ids.Contains(l.UserId)).ToListAsync();
        }

        public Task ClearAllLocationsAsync()
        {
            return _db.DeleteAllAsync<UserLocation>();
        }
    }
}