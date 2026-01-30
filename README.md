# dotnet client library for the Braze Rest Api

NOTE: unoffical and not affiliated with Braze in any way.

## Usage

```cs

service.AddBrazeApi();


var userDataClient = provider.GetRequiredService<IUserDataClient>();

var trackRequestModel = new TrackRequestModel()
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
                { "yolo", new PropertyRequestModel.Literal() { Value = new PropertyLiteral.Integer() { Value = 42, } , }},
                { "yolo2", new PropertyRequestModel.Literal() { Value = new PropertyLiteral.Float() { Value = 42.42, } , }},
                { "yolo3", new PropertyRequestModel.IncrementInteger() { IncrementValue = 42, } },
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
                { "thihi", PropertyLiteral.Create(DateTimeOffset.Parse("2004-01-01T00:00:00+00:00")) },
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
                { "foobar", PropertyLiteral.Create("FOOBAR") }
            },
        },
    ]
};

await userDataClient.Track(trackRequestModel, CancellationToken.None);


```
