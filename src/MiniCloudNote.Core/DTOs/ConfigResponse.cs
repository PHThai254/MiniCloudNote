namespace MiniCloudNote.Core.DTOs
{
    public class ConfigResponse
    {
        public required string EnvironmentName { get; set; }
        public required string ConnectionString { get; set; }
    }
}