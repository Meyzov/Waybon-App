using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class SessionService(IPreferencesService preferences) : ISessionService
    {
        private readonly IPreferencesService _preferences = preferences;

        private const string SessionIdKey = "waybon_sessionId";
        private const string UserIdKey = "waybon_userId";

        public bool IsAuthenticated => SessionId != Guid.Empty;
        public Guid SessionId => Guid.TryParse(_preferences.Get(SessionIdKey), out var id) ? id : Guid.Empty;
        public Guid UserId => Guid.TryParse(_preferences.Get(UserIdKey), out var id) ? id : Guid.Empty;
        public void ClearSession() => _preferences.Clear();
    }
}