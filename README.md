# .NET client library for the Braze Rest Api

This nuget package provides strongly typed API clients for [Braze's API](https://www.braze.com/docs/api/home).

NOTE: unofficial and not affiliated with Braze in any way.

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
                { "yolo_implicit", 42 },
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
                { "foobar", 24.2 },
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

## Development

The clients implemented in this package tries to replicate the logical structure in the [Braze API documentation](https://www.braze.com/docs/api/basics#braze-rest-api-collection).

| Feature                  | Description                                                                 | Status                                       |
|--------------------------|-----------------------------------------------------------------------------|----------------------------------------------|
| Catalogs                 | Create and manage catalogs and catalog items to reference in your Braze campaigns. |                                              |
| Cloud Data Ingestion     | Manage your data warehouse integrations and syncs.                          |                                              |
| Email lists and addresses| Set up and manage bi-directional sync between Braze and your email systems. |                                              |
| Export                   | Access and export various details of your campaigns, Canvases, KPIs, and more. |                                              |
| Messages                 | Schedule, send, and manage your campaigns and Canvases.                     | Partially implmented (API-triggered campaign) |
| Preference center        | Build your preference center and update the styling of it.                  |                                              |
| SCIM                     | Manage user identities in cloud-based applications and services.            |                                              |
| SMS                      | Manage your users’ phone numbers in your subscription groups.               |                                              |
| Subscription groups      | List and update both SMS and email subscription groups stored in the Braze dashboard. |                                              |
| Templates                | Create and update templates for email messaging and Content Blocks.         |                                              |
| User data                | Identify, track, and manage your users.                                     | Partially implemented (track)   |
