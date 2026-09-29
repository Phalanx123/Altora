using System.Text.Json;
using System.Text.Json.Serialization;

namespace Altora.Converters;

/// <summary>
/// Altora carries flags as the strings "1" and "0"; this reads either that or a JSON boolean and
/// writes the string form, leaving a null as null so an omitted flag stays omitted.
/// </summary>
public class NullableJsonBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.GetInt32() == 1,
            JsonTokenType.String => reader.GetString() == "1",
            _ => throw new JsonException($"Unexpected token {reader.TokenType} when parsing a flag.")
        };

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value ? "1" : "0");
    }
}
