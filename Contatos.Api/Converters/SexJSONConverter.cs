using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contatos.Api.Converters
{
    public class SexJsonConverter : JsonConverter<Sex>
    {
        public override Sex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToLower();

            return value switch
            {
                "m" or "male" => Sex.Male,
                "f" or "female" => Sex.Female,
                "o" or "other" => Sex.Other,
                _ => throw new JsonException("Invalid value for Sex. Use 'M'/'Male' or 'F'/'Female' or 'O'/'OTHER'.")
            };
        }

        public override void Write(Utf8JsonWriter writer, Sex value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
