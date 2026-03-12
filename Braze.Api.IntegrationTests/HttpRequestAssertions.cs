using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Helper methods for inspecting and asserting on HTTP requests.
/// </summary>
internal static class HttpRequestAssertions
{
    /// <summary>
    /// Assert that the request has the expected HTTP method.
    /// </summary>
    public static void AssertMethod(this HttpRequestMessage request, HttpMethod expectedMethod)
    {
        Assert.Equal(expectedMethod, request.Method);
    }

    /// <summary>
    /// Assert that the request has the expected relative URI.
    /// </summary>
    public static void AssertUri(this HttpRequestMessage request, string expectedRelativeUri)
    {
        Assert.NotNull(request.RequestUri);
        // Request URI can be absolute or relative depending on HttpClient configuration
        var actualUri = request.RequestUri.IsAbsoluteUri
            ? request.RequestUri.PathAndQuery
            : request.RequestUri.ToString();
        Assert.Equal(expectedRelativeUri, actualUri);
    }

    /// <summary>
    /// Assert that the request has an Authorization header with Bearer token.
    /// </summary>
    public static void AssertBearerToken(this HttpRequestMessage request, string expectedToken)
    {
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("Bearer", request.Headers.Authorization.Scheme);
        Assert.Equal(expectedToken, request.Headers.Authorization.Parameter);
    }

    /// <summary>
    /// Assert that the request has the expected Content-Type header.
    /// </summary>
    public static void AssertContentType(this HttpRequestMessage request, string expectedContentType)
    {
        Assert.NotNull(request.Content);
        Assert.NotNull(request.Content.Headers.ContentType);
        Assert.Equal(expectedContentType, request.Content.Headers.ContentType.MediaType);
    }

    /// <summary>
    /// Read and deserialize the request body as JSON.
    /// </summary>
    public static async Task<JsonDocument> ReadBodyAsJson(this HttpRequestMessage request)
    {
        Assert.NotNull(request.Content);
        var content = await request.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    /// <summary>
    /// Read the request body as a string.
    /// </summary>
    public static async Task<string> ReadBodyAsString(this HttpRequestMessage request)
    {
        Assert.NotNull(request.Content);
        return await request.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Assert that a JSON property exists and has the expected value.
    /// </summary>
    public static void AssertJsonProperty(JsonElement element, string propertyName, JsonElement expectedValue)
    {
        Assert.True(element.TryGetProperty(propertyName, out var property),
            $"Property '{propertyName}' not found in JSON");
        Assert.True(JsonElement.DeepEquals(expectedValue, property));
    }

    /// <summary>
    /// Assert that a JSON property exists and has the expected value.
    /// </summary>
    public static void AssertJsonProperty(JsonElement element, string propertyName, string expectedValue)
    {
        Assert.True(element.TryGetProperty(propertyName, out var property),
            $"Property '{propertyName}' not found in JSON");
        Assert.Equal(expectedValue, property.GetString());
    }

    /// <summary>
    /// Assert that a JSON property exists and has the expected integer value.
    /// </summary>
    public static void AssertJsonProperty(JsonElement element, string propertyName, int expectedValue)
    {
        Assert.True(element.TryGetProperty(propertyName, out var property),
            $"Property '{propertyName}' not found in JSON");
        Assert.Equal(expectedValue, property.GetInt32());
    }

    /// <summary>
    /// Assert that a JSON property exists and has the expected boolean value.
    /// </summary>
    public static void AssertJsonProperty(JsonElement element, string propertyName, bool expectedValue)
    {
        Assert.True(element.TryGetProperty(propertyName, out var property),
            $"Property '{propertyName}' not found in JSON");
        Assert.Equal(expectedValue, property.GetBoolean());
    }

    /// <summary>
    /// Assert that a JSON property exists.
    /// </summary>
    public static void AssertJsonPropertyExists(JsonElement element, string propertyName)
    {
        Assert.True(element.TryGetProperty(propertyName, out _),
            $"Property '{propertyName}' not found in JSON");
    }

    /// <summary>
    /// Assert that a JSON property does not exist.
    /// </summary>
    public static void AssertJsonPropertyDoesNotExist(JsonElement element, string propertyName)
    {
        Assert.False(element.TryGetProperty(propertyName, out _),
            $"Property '{propertyName}' should not exist in JSON");
    }
}
