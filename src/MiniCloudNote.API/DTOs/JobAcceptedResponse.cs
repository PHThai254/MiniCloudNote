namespace MiniCloudNote.API.DTOs
{
    public class JobAcceptedResponse
    {
        public string? Message { get; set; } 
        public required string JobId { get; set; } 
        public string? Note { get; set; } 
    }
}