using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        public Task GoToAsync(string route) => Shell.Current.GoToAsync(route);
    }
}