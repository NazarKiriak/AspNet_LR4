using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder();

builder.Services.AddTransient<Book>();
builder.Services.AddTransient<UserInfo>();

builder.Configuration.AddJsonFile("lr4/Books.json");

var app = builder.Build();
var configuration= app.Services.GetService<IConfiguration>();
var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "lr4/users");

if (File.Exists(jsonFilePath))
{
    var jsonContent = File.ReadAllText(jsonFilePath);
}

app.Map("/", () => "Index Page");

app.Map("/Library", async (context) =>
{
    await context.Response.WriteAsync("Hello reader!");
});

app.Map("/Library/books", async (context) =>
{
    var book = app.Services.GetService<Book>();
    var booksList = configuration.GetSection("Books").Get<List<Book>>();
    foreach (var item in booksList)
    {
        await context.Response.WriteAsync($"Book: {item.Title} by {item.Author}\n");
    }
});

app.Map("/Library/Profile/{id:int?}", (int? id) =>
{
    if (id.HasValue && id >= 0 && id <= 5)
    {
        var configFileName = $"lr4/users/user{id}.json";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);

        if (File.Exists(filePath))
        {
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(File.ReadAllText(filePath));
            return $"Name: {userInfo.Name}, Age: {userInfo.Age}";
        }
        else
        {
            return "File configuration of user is not found.";
        }
    }
    else
    {
  
        var defaultUserInfo = JsonConvert.DeserializeObject<UserInfo>(File.ReadAllText("user0.json"));
        return $"Name: {defaultUserInfo.Name}, Age: {defaultUserInfo.Age}";
    }

});

app.MapGet("/allmaps", (IEnumerable<EndpointDataSource> endpointSources) =>
string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));

app.Run();
