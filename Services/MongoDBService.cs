using MongoDB.Driver;
using System.Collections;
using System.Xml.Linq;

namespace AISummariseApplication.Services
{
    public class MongoDBService 
    {
        private readonly IMongoDatabase Database;
        private MongoClient _MongoClient; 
        

        public MongoDBService(IConfiguration configuration)
        {
            var connectionstring = configuration.GetConnectionString("MongoDB");
            var MongoDBClient = new MongoClient(connectionstring);
            _MongoClient = MongoDBClient;
            Database = MongoDBClient.GetDatabase("Test");
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return Database.GetCollection<T>(collectionName);
        }

        public IMongoDatabase GetDatabase(string Name)
        {
            return _MongoClient.GetDatabase(Name);
        }

    }
}
