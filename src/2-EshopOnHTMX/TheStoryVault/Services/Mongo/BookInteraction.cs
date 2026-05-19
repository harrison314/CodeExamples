using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TheStoryVault.Services.Mongo;

public class BookInteraction
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string UserId { get; set; }

    public int BookId { get; set; }
    public double Weigth { get; set; }
    public string InteractionType { get; set; }
    public DateTime Time { get; set; }

}