using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TheStoryVault;

public class MongoRegister
{
    public static void RegisterMongodb(WebApplicationBuilder builder)
    {
        builder.Services.Configure<MongoDatabaseSetup>(builder.Configuration.GetSection("MongoDatabaseSetup"));
        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            IOptions<MongoDatabaseSetup> setup = sp.GetRequiredService<IOptions<MongoDatabaseSetup>>();
            return new MongoClient(setup.Value.ConnectionString);
        });

        builder.Services.AddSingleton<IMongoDatabase>(sp =>
        {
            IOptions<MongoDatabaseSetup> setup = sp.GetRequiredService<IOptions<MongoDatabaseSetup>>();
            IMongoClient client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(setup.Value.DatabaseName);
        });
    }
}