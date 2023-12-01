using System.Text.Json;
using Amazon.Runtime;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService;
using SearchService.Services;

public class DBInitializer
{
    public static async Task InitDB(WebApplication app)
    {
        await DB.InitAsync("SearchDB",MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();
        
        using var scope = app.Services.CreateScope();

        var htttClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await htttClient.GetItemsForSearchDB();

        Console.WriteLine(items.Count);

        if(items.Count > 0)
        {
            await DB.SaveAsync(items);
        }
    }
}