using SQLite;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;

namespace Waybon.App.Data.Repositories
{
    public class GroupMemberRepository(IDatabaseService database) : IGroupMemberRepository
    {
        private readonly SQLiteAsyncConnection _db = database.Connection;

        public async Task SaveMembersAsync(int groupId, IEnumerable<LocalMember> members)
        {
            await _db.RunInTransactionAsync
            (
                tran =>
                {
                    tran.Execute("DELETE FROM LocalMember WHERE GroupId = ?", groupId);
                    tran.InsertAll(members);
                }
            );
        }

        public Task<List<LocalMember>> GetMembersAsync(int groupId)
        {
            return _db.Table<LocalMember>().Where(m => m.GroupId == groupId).ToListAsync();
        }

        public Task ClearMembersAsync(int groupId)
        {
            return _db.ExecuteAsync("DELETE FROM LocalMember WHERE GroupId = ?", groupId);
        }

        public Task ClearAllMembersAsync()
        {
            return _db.DeleteAllAsync<LocalMember>();
        }
    }
}