using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Altora.Converters;

public class CustomDateTimeConverter : JsonConverter<DateTime?>
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return DateTime.ParseExact(reader.GetString() ?? throw new InvalidOperationException(), DateTimeFormat,
            CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString(DateTimeFormat));
        }
    }
}