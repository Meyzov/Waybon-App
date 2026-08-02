namespace Waybon.App.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoToAsync(string route);
    }
}