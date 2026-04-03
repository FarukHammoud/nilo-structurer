namespace Server {
    using Domain;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class ContractConverter : JsonConverter<Contract> {

        private readonly Dictionary<string, Type> _contractKinds = new()
        {
            { "cash_flow", typeof(CashFlow) },
            { "european_call_option", typeof(EuropeanCall) }
            // you can extend this map
        };

        public override Contract Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("kind", out var kindProp)) {
                throw new JsonException("Missing kind discriminator");
            }

            string kind = kindProp.GetString();

            if (!_contractKinds.TryGetValue(kind, out var targetType)) {
                throw new JsonException($"Unknown kind '{kind}'");
            }

            return (Contract)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
        }

        public override void Write(Utf8JsonWriter writer, Contract value, JsonSerializerOptions options) {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
