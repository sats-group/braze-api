using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api;

[JsonConverter(typeof(ApiResponseJsonConverterFactory))]
internal sealed class InternalApiResponse<T>
{
    public required string Message { get; init; }

    public List<JsonElement>? Errors { get; init; }

    public T? Value { get; init; }
}

internal sealed class ApiResponseJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
           && typeToConvert.GetGenericTypeDefinition() == typeof(InternalApiResponse<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var t = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ApiResponseJsonConverter<>).MakeGenericType(t);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class ApiResponseJsonConverter<T> : JsonConverter<InternalApiResponse<T>>
    {
        public override InternalApiResponse<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Expect an object at the root
            using var doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("Expected a JSON object.");
            }

            var root = doc.RootElement;


            if (!root.TryGetProperty("message", out var messageEl)
                || messageEl.ValueKind != JsonValueKind.String
                || messageEl.GetString() is not { } message)
            {
                throw new JsonException("Required 'message' strings.");
            }

            List<JsonElement>? errors = null;
            if (root.TryGetProperty("errors", out var errorsEl))
            {
                switch (errorsEl.ValueKind)
                {
                    case JsonValueKind.Array:
                        errors = errorsEl
                            .EnumerateArray()
                            .Select(static i => i.Clone())
#pragma warning disable IDE0305
                            .ToList();
#pragma warning restore IDE0305
                        break;
                    case JsonValueKind.Null:
                        break;
                    default:
                        throw new JsonException($"'errors' must be array or null ({errorsEl.ValueKind}).");
                }
            }


            var value = root.Deserialize<T>();
            return new InternalApiResponse<T>
            {
                Message = message,
                Errors = errors,
                Value = value,
            };
        }

        public override void Write(Utf8JsonWriter writer, InternalApiResponse<T> value, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }
}
