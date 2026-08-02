using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class PreferencesService : IPreferencesService
    {
        public void Set(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public string Get(string key, string defaultValue = "")
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Clear()
        {
            Preferences.Clear();
        }
    }
}
