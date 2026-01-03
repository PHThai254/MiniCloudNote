namespace MiniCloudNote.API.DTOs
{
    public class ConfigResponse
    {
        public required string EnvironmentName { get; set; }
        public required string ConnectionString { get; set; }
    }
}