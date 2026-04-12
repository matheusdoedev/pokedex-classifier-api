using System.Text.Json.Serialization;

internal record ModelGenerateResponse(
    [property: JsonPropertyName("response")]
    string Response,

    [property: JsonPropertyName("model")]
    string Model,

    [property: JsonPropertyName("created_at")]
    DateTime CreatedAt,

    [property: JsonPropertyName("done")]
    bool Done
);
