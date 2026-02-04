using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.Tests;

public class ApiResponseSerializationTests
{
    [Fact]
    public async Task CreateApiResponseBrazeException()
    {
        var json = """
                   {
                     "attributes_processed": 1,
                     "message": "Valid data must be provided in the 'attributes', 'events', or 'purchases' fields.",
                     "errors": [
                       {
                         "type": "'external_id', 'braze_id', 'user_alias', 'email' or 'phone' is required",
                         "input_array": "attributes",
                         "index": 0
                       }
                     ]
                   }
                   """;
        var responseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(json),
            StatusCode = System.Net.HttpStatusCode.BadRequest,
        };

        var exception = await Assert.ThrowsAsync<BrazeApiException>(async () =>
            await responseMessage.CreateApiResponse<TrackResponse>(CancellationToken.None));
        Assert.Equal(
            "Valid data must be provided in the 'attributes', 'events', or 'purchases' fields.",
            exception.Message);
        Assert.NotNull(exception.Errors);
        Assert.Single(exception.Errors);
    }

    [Fact]
    public async Task CreateApiResponseNonFatalErrorResponse()
    {
        var json = """
                   {
                     "attributes_processed": 1,
                     "message": "success",
                     "errors": [
                       {
                         "type": "‘external_id’, ‘braze_id’, ‘user_alias’, ‘email’ or ‘phone’ is required",
                         "input_array": "attributes",
                         "index": 1
                       }
                     ]
                   }
                   """;
        var responseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(json),
            StatusCode = System.Net.HttpStatusCode.OK,
        };

        var response = await responseMessage.CreateApiResponse<TrackResponse>(CancellationToken.None);
        Assert.NotNull(response.Value);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Single(response.NonFatalErrors);
        Assert.Equal(1, response.Value.AttributesProcessed);
        Assert.Null(response.Value.EventsProcessed);
        Assert.Null(response.Value.PurchasesProcessed);
    }

    [Fact]
    public async Task CreateApiResponseSuccessResponseErrorsNull()
    {
        var json = """
                   {
                     "attributes_processed": 1,
                     "message": "success",
                     "errors": null
                   }
                   """;
        var responseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(json),
            StatusCode = System.Net.HttpStatusCode.OK,
        };

        var response = await responseMessage.CreateApiResponse<TrackResponse>(CancellationToken.None);
        Assert.NotNull(response.Value);
        Assert.Null(response.NonFatalErrors);
        Assert.Equal(1, response.Value.AttributesProcessed);
        Assert.Null(response.Value.EventsProcessed);
        Assert.Null(response.Value.PurchasesProcessed);
    }

    [Fact]
    public async Task CreateApiResponseSuccessResponse()
    {
        var json = """
                   {
                     "attributes_processed": 1,
                     "message": "success"
                   }
                   """;
        var responseMessage = new HttpResponseMessage()
        {
            Content = new StringContent(json),
            StatusCode = System.Net.HttpStatusCode.OK,
        };

        var response = await responseMessage.CreateApiResponse<TrackResponse>(CancellationToken.None);
        Assert.NotNull(response.Value);
        Assert.Null(response.NonFatalErrors);
        Assert.Equal(1, response.Value.AttributesProcessed);
        Assert.Null(response.Value.EventsProcessed);
        Assert.Null(response.Value.PurchasesProcessed);
    }
}
