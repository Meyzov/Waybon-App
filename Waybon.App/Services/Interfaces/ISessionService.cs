namespace Waybon.App.Services.Interfaces
{
    public interface ISessionService
    {
        bool IsAuthenticated { get; }
        Guid SessionId { get; }
        Guid UserId { get; }

        void ClearSession();
    }
}