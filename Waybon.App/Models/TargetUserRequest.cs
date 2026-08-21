namespace Waybon.App.Models
{
    public class TargetUserRequest
    {
        public Guid SessionId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}