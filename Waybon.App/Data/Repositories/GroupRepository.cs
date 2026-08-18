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

        public async Task<IEnumerable<LocalGroup>> GetGroupsAsync()
        {
            return await _db.Table<LocalGroup>().ToListAsync();
        }

        public async Task<LocalGroup?> GetGroupByIdAsync(int groupId)
        {
            return await _db.Table<LocalGroup>().Where(g => g.GroupId == groupId).FirstOrDefaultAsync();
        }

        public Task SaveGroupAsync(LocalGroup group)
        {
            return _db.InsertOrReplaceAsync(group);
        }

        public Task ClearAllGroupsAsync()
        {
            return _db.DeleteAllAsync<LocalGroup>();
        }
    }
}