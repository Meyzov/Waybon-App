using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class SessionService(IPreferencesService preferences) : ISessionService
    {
        private readonly IPreferencesService _preferences = preferences;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_preferences.Get("waybon_sessionId"));

        public void ClearSession()
        {
            _preferences.Clear();
        }
    }
}