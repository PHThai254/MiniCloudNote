namespace MiniCloudNote.API.DTOs
{
    public class ServerInfoResponse
    {
        public string? Message { get; set; }
        public required string ServerId { get; set; }
    }
}