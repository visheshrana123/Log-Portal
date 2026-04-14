using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogPortal.API.Models
{
   
    public class LogEntry
    {
        [BsonId]  
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        
        public string ServerId { get; set; } = null!;

       
        public string Level { get; set; } = null!;

       
        public string Message { get; set; } = null!;

      
        public string Source { get; set; } = string.Empty;

       
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsAnomaly { get; set; } = false;
    }
}