using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogPortal.API.Models
{
    
    public class Anomaly
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

                public string Type { get; set; } = null!;

        
        public string RuleTriggered { get; set; } = null!;

      
        public string Severity { get; set; } = null!; // LOW, MEDIUM, HIGH, CRITICAL

        
        public string ServerId { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

       
        public DateTime DetectedAt { get; set; } = DateTime.UtcNow;

        
        public bool AlertSent { get; set; } = false;
    }
}