using LogPortal.API.Data;
using LogPortal.API.Models;
using MongoDB.Driver;

namespace LogPortal.API.Services
{
    public class AlertService
    {
        private readonly MongoDbContext _context;

        public AlertService(MongoDbContext context)
        {
            _context = context;
        }

      
        public async Task SendAlertAsync(Anomaly anomaly)
        {
            
            Console.WriteLine("🚨 ALERT TRIGGERED!");
            Console.WriteLine($"Type: {anomaly.Type}");
            Console.WriteLine($"Server: {anomaly.ServerId}");
            Console.WriteLine($"Message: {anomaly.Description}");

            
            var update = Builders<Anomaly>.Update
                .Set(a => a.AlertSent, true);

            await _context.Anomalies.UpdateOneAsync(
                a => a.Id == anomaly.Id,
                update);
        }
    }
}