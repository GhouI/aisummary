using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AISummariseApplication.Services.DatabaseClasses
{
    [BsonIgnoreExtraElements]
    public class Comment
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement("name")]
        public BsonString Name { get; set; }
        [BsonElement("email")]
        public BsonString Email { get; set; }
        [BsonElement("movie_id")]
        public ObjectId MovieId { get; set; }
        [BsonElement("text")]
        public BsonString Text { get; set; }
        [BsonElement("date")]
        public BsonDateTime Date;
    }
}
