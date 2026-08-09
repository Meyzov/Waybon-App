using SQLite;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;

namespace Waybon.App.Data.Repositories
{
    public class GroupRepository(IDatabaseService database) : IGroupRepository
    {
        private readonly SQLiteAsyncConnection _db = database.Connection;

        public async Task SaveGroupsAsync(IEnumerable<LocalGroup> groups)
        {
            await _db.RunInTransactionAsync
            (
                tran =>
                {
                    tran.DeleteAll<LocalGroup>();
                    tran.InsertAll(groups);
                }
            );
        }

        public Task<List<LocalGroup>> GetGroupsAsync()
        {
            return _db.Table<LocalGroup>().ToListAsync();
        }

        public Task ClearAllGroupsAsync()
        {
            return _db.DeleteAllAsync<LocalGroup>();
        }
    }
}