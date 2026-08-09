using SQLite;

namespace Waybon.App.Data.Interfaces
{
    public interface IDatabaseService
    {
        SQLiteAsyncConnection Connection { get; }
        Task InitializeAsync();
        Task ClearAllAsync();
    }
}