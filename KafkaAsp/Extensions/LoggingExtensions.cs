using System.Text.Json;

namespace KafkaAsp.Extensions
{
    public static class LoggingExtensions
    {
        public static string Dump(this object? obj)
        {
            return obj is null
                ? string.Empty
                : JsonSerializer.Serialize(obj)
                    .Replace("\\\\u0022", string.Empty)
                    .Replace("\\u0022", string.Empty)
                    .Replace("\\\"", string.Empty)
                    .Replace("\"", string.Empty);
        }
    }
}
