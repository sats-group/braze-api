using System.Collections.Generic;
using System.Text.Json;

namespace Braze.Api;

/// <summary>
/// Represents an error response from the Braze API.
/// </summary>
internal class BrazeErrorResponse
{
    public string? Message { get; init; }
    public List<JsonElement>? Errors { get; init; }
}
