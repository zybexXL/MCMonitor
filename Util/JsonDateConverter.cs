using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MCMonitor
{

    // produce sortable dateTime strings like "2021-06-12T19:24:20"
    public class JsonDateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("s"));
        }
    }
}
