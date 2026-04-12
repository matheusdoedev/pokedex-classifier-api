using System.Text.Json.Serialization;

internal record ModelGenerateRequest(
    [property: JsonPropertyName("model")]
    string Model,

    [property: JsonPropertyName("prompt")]
    string Prompt,

    [property: JsonPropertyName("stream")]
    bool Stream = false
);
