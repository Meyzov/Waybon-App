namespace Waybon.App.Models
{
    public class LoginResponse
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool SharingEnabled { get; set; }
    }
}