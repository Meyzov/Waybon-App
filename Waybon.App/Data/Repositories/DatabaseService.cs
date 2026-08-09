using SQLite;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;

namespace Waybon.App.Data.Repositories
{
    public class DatabaseService : IDatabaseService
    {
        public SQLiteAsyncConnection Connection { get; }

        public DatabaseService()
        {
            var dbPath = Path.Combine
            (
                FileSystem.AppDataDirectory, "waybon.db3"
            );

            Connection = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync()
        {
            await Connection.CreateTableAsync<LocalGroup>();
            await Connection.CreateTableAsync<LocalMember>();
            await Connection.CreateTableAsync<UserLocation>();
        }

        public async Task ClearAllAsync()
        {
            await Connection.DeleteAllAsync<LocalGroup>();
            await Connection.DeleteAllAsync<LocalMember>();
            await Connection.DeleteAllAsync<UserLocation>();
        }
    }
}