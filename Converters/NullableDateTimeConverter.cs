using System.Text.Json;
using System.Text.Json.Serialization;

namespace IMSWeb.Utils;

/// <inheritdoc />
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            if (DateTime.TryParse(stringValue, out var date))
            {
                return date;
            }
        }
        throw new JsonException("Invalid date format");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString("yyyy-MM-dd"));
    }
}