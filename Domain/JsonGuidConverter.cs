namespace TradingPartnerManagement.Domain;

using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.7.2.0 (Newtonsoft.Json v13.0.0.0)")]
internal class JsonGuidConverter : System.Text.Json.Serialization.JsonConverter<System.Guid>
{
    public override Guid Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (String.IsNullOrEmpty(value) || reader.GetGuid() == Guid.Empty)
        {
            throw new System.Text.Json.JsonException("Invalid Guid");
        }
        return reader.GetGuid();
    }
    public override void Write(System.Text.Json.Utf8JsonWriter writer, System.Guid value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
} 