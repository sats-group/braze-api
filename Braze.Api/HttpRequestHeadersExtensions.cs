using System.Linq;
using System.Net.Http.Headers;

namespace Braze.Api;

internal static class HttpRequestHeadersExtensions
{
    public static int GetIntOrDefault(this HttpResponseHeaders headers, string headerName)
    {
        if (headers.TryGetValues(headerName, out var values)
            && values.FirstOrDefault() is { } value
            && int.TryParse(value, out var result))
        {
            return result;
        }

        return 0;
    }
}
