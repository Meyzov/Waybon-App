namespace Waybon.App.Models
{
    public class CreateGroupRequest
    {
        public Guid SessionId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
