namespace Waybon.App.Models
{
    public class UpdateSharingRequest
    {
        public Guid SessionId { get; set; }
        public bool SharingEnabled { get; set; }
    }
}
