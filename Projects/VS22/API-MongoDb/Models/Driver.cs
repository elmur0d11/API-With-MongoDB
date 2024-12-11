using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_MongoDb.Models
{
    public class Driver
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? DriverName { get; set; } = null;

        public int Number { get; set; }

        public string Team { get; set; } = null;
    }
}
