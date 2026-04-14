using LogPortal.API.Data;
using LogPortal.API.Models;
using MongoDB.Driver;

namespace LogPortal.API.Services
{
    public class AnomalyDetectionService
    {
        private readonly MongoDbContext _context;
        private readonly AlertService _alertService; 

        
        public AnomalyDetectionService(
            MongoDbContext context,
            AlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        
        public async Task DetectAnomaliesAsync(LogEntry newLog)
        {
            Console.WriteLine(" Anomaly detection running...");

            await DetectErrorSpike(newLog);
        }

        
        private async Task DetectErrorSpike(LogEntry log)
        {
            if (log.Level != "ERROR")
                return;

            var errorCount = await _context.Logs.CountDocumentsAsync(
                l => l.Level == "ERROR");

            Console.WriteLine($"Total ERROR logs: {errorCount}");

            if (errorCount >= 3)
            {
                
                var recentTime = DateTime.UtcNow.AddMinutes(-5);

                var existing = await _context.Anomalies.Find(a =>
                    a.Type == "ErrorSpike" &&
                    a.ServerId == log.ServerId &&
                    a.DetectedAt >= recentTime
                ).FirstOrDefaultAsync();

                if (existing != null)
                {
                    Console.WriteLine(" Anomaly already exists, skipping...");
                    return;
                }

                var anomaly = new Anomaly
                {
                    Type = "ErrorSpike",
                    RuleTriggered = "3+ errors detected",
                    Severity = "HIGH",
                    ServerId = log.ServerId,
                    Description = $"Error spike detected: {errorCount} errors",
                    DetectedAt = DateTime.UtcNow,
                    AlertSent = false
                };

                
                await _context.Anomalies.InsertOneAsync(anomaly);

                Console.WriteLine(" Anomaly inserted into DB!");

                
                await _alertService.SendAlertAsync(anomaly);
            }
        }
    }
}