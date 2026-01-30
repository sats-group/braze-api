# .NET client library for the Braze Rest Api

NOTE: unoffical and not affiliated with Braze in any way.

## Usage

Configuration (e.g. appsettings.json):
```json
{
  "Braze": {
    "BaseAddress": "https://rest.fra-02.braze.eu",
    "ApiKey": "<SECRET KEY>"
  }
}
```

```cs

service.AddBrazeApi();
// or as keyed services
// service.AddBrazeApi("KEY-A", "Braze:A");
// service.AddBrazeApi("KEY-B", "Braze:B");
```

Client usage:

```cs

var userDataClient = provider.GetRequiredService<IUserDataClient>();

// or via IBrazeProviderFactory (usful for keyed services)
// var factory = provider.GetRequiredService<IBrazeProviderFactory>();
// var brazeProvider = factory.Get("KEY-A");
// var userDataClient = brazeProvider.UserDataClient;

// or directly via keyed services
// var userDataClient = provider.GetRequiredKeyedService<IUserDataClient>("KEY-A");

var track = new Track()
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

await userDataClient.Track(track, CancellationToken.None);
```
