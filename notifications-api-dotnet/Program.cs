using System.Reflection;
using System.Text.Json;
using NotificationApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Storage.Seed();
var processor = new NotificationProcessor();

app.MapPost("/notifications", (CreateNotificationRequest req) =>
{
    var n = Storage.AddNotification(req.TargetChannels, req.Message);
    return Results.Json(n);
});

app.MapGet("/notifications", () => Results.Json(Storage.GetAll()));

app.MapGet("/notifications/{id:int}", (int id) =>
{
    var n = Storage.FindById(id);
    if (n == null) return Results.Json(new { error = "not found" }, statusCode: 404);
    return Results.Json(n);
});

app.MapPut("/notifications/{id:int}", async (int id, HttpRequest request) =>
{
    var n = Storage.FindById(id);
    if (n == null) return Results.Json(new { error = "not found" }, statusCode: 404);
    var updates = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(request.Body);
    foreach (var kvp in updates!)
    {
        var prop = typeof(Notification).GetProperty(kvp.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            var value = kvp.Value.Deserialize(prop.PropertyType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            prop.SetValue(n, value);
        }
    }
    return Results.Json(n);
});

app.MapPost("/notifications/{id:int}/send", (int id) =>
{
    var n = Storage.FindById(id);
    if (n == null) return Results.Json(new { error = "not found" }, statusCode: 404);
    processor.SendOne(n);
    return Results.Json(n);
});

app.MapPost("/notifications/send-bulk", () =>
{
    processor.SendAll();
    return Results.Json(Storage.GetAll());
});

app.Run("http://localhost:3000");
