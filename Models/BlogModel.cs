using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AISummariseApplication.Models
{
    public class BlogModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("Title")]
        public BsonString Title { get; set; }
        [BsonElement("URL")]
        public BsonString URL { get; set; }
        [BsonElement("Summary")]
        public BsonString Summary { get; set; }
        [BsonElement("CreatedAt")]
        public BsonDateTime Date { get; set; }

 
    }
}
