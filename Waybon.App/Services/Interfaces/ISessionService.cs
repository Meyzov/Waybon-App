namespace Waybon.App.Services.Interfaces
{
    public interface ISessionService
    {
        bool IsAuthenticated { get; }
        void ClearSession();
    }
}
