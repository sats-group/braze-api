using System;
using System.Text.Json;
using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.Tests;

public class UserDataSerializationTests
{
    [Fact]
    public void SerializeTrackRequestModel()
    {
        var track = new TrackRequest()
        {
            Attributes =
            [
                new()
                {
                    BrazeId = "72BBAA5C-B137-4895-8735-B163A34CD133",
                    Country = "NO",
                    DateOfBirth = new DateOnly(2000, 12, 10),
                    DateOfFirstSession = DateTimeOffset.Parse("2001-01-01T00:00:00Z"),
                    Gender = Gender.PreferNotToSay,
                    CustomAttributes = new ()
                    {
                        { "yolo", PropertyOp.Literal(42) },
                        { "yolo2", PropertyOp.Literal(42.42) },
                        { "yolo3", new PropertyOp.IncrementInteger() { IncrementValue = 42, } },
                        { "yolo5", (string?)null },
                    },
                }
            ],
            Events =
            [
                new ()
                {
                    Name = "navn",
                    Time = DateTimeOffset.Parse("2003-01-01T00:00:00+00:00"),
                    Email = "yolo@foobar.com",
                    Properties = new ()
                    {
                        { "thihi", Property.Create(DateTimeOffset.Parse("2004-01-01T00:00:00+00:00")) },
                        { "foobar", 24.2 }
                    },
                },
            ],
            Purchases =
            [
                new()
                {
                    ProductId = "123",
                    Currency = "NOK",
                    Price = 42.42M,
                    Time = DateTimeOffset.Parse("2002-01-01T00:00:00+00:00"),
                    Properties = new ()
                    {
                        { "foobar", Property.Create("FOOBAR") }
                    },
                },
            ]
        };


        var serialized = JsonSerializer.Serialize(
            track,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var actual = JsonDocument.Parse(serialized).RootElement;
        var expected = JsonDocument
            .Parse(
                """
                {
                  "attributes": [
                    {
                      "braze_id": "72BBAA5C-B137-4895-8735-B163A34CD133",
                      "country": "NO",
                      "date_of_first_session":"2001-01-01T00:00:00+00:00",
                      "dob":"2000-12-10",
                      "yolo": 42,
                      "yolo2": 42.42,
                      "yolo3": { "inc": 42 },
                      "gender": "P",
                      "yolo5": null
                    }
                  ],
                  "purchases": [
                    {
                      "product_id": "123",
                      "currency": "NOK",
                      "price": 42.42,
                      "time": "2002-01-01T00:00:00+00:00",
                      "properties": {
                        "foobar": "FOOBAR"
                      }
                    }
                  ],
                  "events": [
                    {
                      "name": "navn",
                      "time": "2003-01-01T00:00:00+00:00",
                      "email": "yolo@foobar.com",
                      "properties": {
                        "thihi": "2004-01-01T00:00:00+00:00",
                        "foobar": 24.2
                      }
                    }
                  ]
                }
                """)
            .RootElement;

        Assert.True(JsonElement.DeepEquals(expected, actual), $"'{actual}' not equal to '{expected}'");
    }
}
