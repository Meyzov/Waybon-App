using SQLite;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;

namespace Waybon.App.Data.Repositories
{
    public class GroupMemberRepository(IDatabaseService database) : IGroupMemberRepository
    {
        private readonly SQLiteAsyncConnection _db = database.Connection;

        public Task SaveUserAsync(LocalUser user) => _db.InsertOrReplaceAsync(user);

        public async Task SaveMembersAsync(int groupId, IEnumerable<LocalUser> users)
        {
            await _db.RunInTransactionAsync(tran =>
            {
                tran.Execute("DELETE FROM LocalGroupMember WHERE GroupId = ?", groupId);

                foreach (var user in users)
                {
                    tran.InsertOrReplace(user);
                    tran.Insert
                    (
                        new LocalGroupMember
                        {
                            GroupId = groupId,
                            UserId = user.UserId
                        }
                    );
                }
            });
        }

        public async Task<IEnumerable<LocalUser>> GetMembersAsync(int groupId)
        {
            var query = "SELECT U.* FROM LocalUser AS U JOIN LocalGroupMember AS GM ON U.UserId = GM.UserId WHERE GM.GroupId = ?";
            return await _db.QueryAsync<LocalUser>(query, groupId);
        }

        public Task ClearMembersAsync(int groupId) => _db.ExecuteAsync("DELETE FROM LocalGroupMember WHERE GroupId = ?", groupId);

        public async Task ClearAllMembersAsync()
        {
            await _db.RunInTransactionAsync(tran =>
            {
                tran.DeleteAll<LocalUser>();
                tran.DeleteAll<LocalGroupMember>();
            });
        }
    }
}