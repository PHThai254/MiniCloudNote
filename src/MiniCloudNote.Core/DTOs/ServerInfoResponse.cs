namespace MiniCloudNote.Core.DTOs
{
    public class ServerInfoResponse
    {
        public string? Message { get; set; }
        public required string ServerId { get; set; }
    }
}