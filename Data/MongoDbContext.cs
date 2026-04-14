using MongoDB.Driver;
using LogPortal.API.Models;

namespace LogPortal.API.Data
{
    
    public class MongoDbContext
    
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            
            var client = new MongoClient(settings.ConnectionString);

            
            _database = client.GetDatabase(settings.DatabaseName);

            
            CreateIndexes();
        }

        
        public IMongoCollection<LogEntry> Logs =>
            _database.GetCollection<LogEntry>("logs");

       
        public IMongoCollection<Anomaly> Anomalies =>
            _database.GetCollection<Anomaly>("anomalies");

        
        public IMongoCollection<Alert> Alerts =>
            _database.GetCollection<Alert>("alerts");

               private void CreateIndexes()
        {
            
            var timestampIndex = Builders<LogEntry>.IndexKeys
                .Ascending(log => log.Timestamp);

            Logs.Indexes.CreateOne(
                new CreateIndexModel<LogEntry>(timestampIndex)
            );

           
            var serverIndex = Builders<LogEntry>.IndexKeys
                .Ascending(log => log.ServerId);

            Logs.Indexes.CreateOne(
                new CreateIndexModel<LogEntry>(serverIndex)
            );

            
            var anomalyIndex = Builders<Anomaly>.IndexKeys
                .Descending(a => a.DetectedAt);

            Anomalies.Indexes.CreateOne(
                new CreateIndexModel<Anomaly>(anomalyIndex)
            );
        }
    }
}