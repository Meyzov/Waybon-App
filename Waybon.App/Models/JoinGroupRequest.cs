namespace Waybon.App.Models
{
    public class JoinGroupRequest
    {
        public Guid SessionId { get; set; }
        public string JoinCode { get; set; } = string.Empty;
    }
}
