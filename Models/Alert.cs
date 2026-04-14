using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogPortal.API.Models
{
   
    public class Alert
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

       
        [BsonRepresentation(BsonType.ObjectId)]
        public string AnomalyId { get; set; } = null!;

        
        public string SentTo { get; set; } = null!;

                public string Message { get; set; } = string.Empty;

        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        
        public string Status { get; set; } = "SENT";
    }
}