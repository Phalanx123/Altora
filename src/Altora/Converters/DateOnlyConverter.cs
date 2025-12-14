using System.Text.Json;
using System.Text.Json.Serialization;

namespace Altora.Converters
{
    public class DateOnlyConverter : JsonConverter<DateOnly?>
    {
        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    var str = reader.GetString();
                    if (string.IsNullOrWhiteSpace(str)) return null;
                    return DateOnly.Parse(str);
                }
                case JsonTokenType.Null:
                    return null;
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType} when parsing DateOnly.");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            else
                writer.WriteNullValue();
        }
    }
}