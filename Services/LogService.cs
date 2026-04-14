using LogPortal.API.Data;
using LogPortal.API.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;

namespace LogPortal.API.Services
{
    public class LogService
    {
        private readonly MongoDbContext _context;
        private readonly AnomalyDetectionService _anomalyService;

        
        public LogService(MongoDbContext context, AnomalyDetectionService anomalyService)
        {
            _context = context;
            _anomalyService = anomalyService;
        }

        
        public async Task<string> IngestLogAsync(LogEntry log)
        {
            if (string.IsNullOrEmpty(log.ServerId))
                return "ServerId is required";

            if (string.IsNullOrEmpty(log.Level))
                return "Log level is required";

            if (string.IsNullOrEmpty(log.Message))
                return "Message is required";

            if (log.Timestamp == default)
                log.Timestamp = DateTime.UtcNow;

            
            await _context.Logs.InsertOneAsync(log);

            
            await _anomalyService.DetectAnomaliesAsync(log);

            return "Log stored successfully";
        }

       
        public async Task<List<LogEntry>> GetAllLogsAsync()
        {
            return await _context.Logs
                .Find(_ => true)
                .SortByDescending(l => l.Timestamp)
                .ToListAsync();
        }

       
        public async Task<List<LogEntry>> GetFilteredLogsAsync(
            string? serverId,
            string? level,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1,
            int pageSize = 10)
        {
            var filter = Builders<LogEntry>.Filter.Empty;

            if (!string.IsNullOrEmpty(serverId))
                filter &= Builders<LogEntry>.Filter.Regex(
                    l => l.ServerId,
                    new BsonRegularExpression(serverId, "i"));

            if (!string.IsNullOrEmpty(level))
                filter &= Builders<LogEntry>.Filter.Eq(l => l.Level, level);

            if (startDate.HasValue)
                filter &= Builders<LogEntry>.Filter.Gte(l => l.Timestamp, startDate.Value);

            if (endDate.HasValue)
                filter &= Builders<LogEntry>.Filter.Lte(l => l.Timestamp, endDate.Value);

            return await _context.Logs
                .Find(filter)
                .SortByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

       
        public async Task<object> GetLogStatsAsync()
        {
            var logs = await _context.Logs.Find(_ => true).ToListAsync();

            var levelStats = logs
                .GroupBy(l => l.Level)
                .Select(g => new
                {
                    Level = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var errorStats = logs
                .Where(l => l.Level == "ERROR")
                .GroupBy(l => new
                {
                    l.Timestamp.Year,
                    l.Timestamp.Month,
                    l.Timestamp.Day,
                    l.Timestamp.Hour
                })
                .Select(g => new
                {
                    Time = $"{g.Key.Year}-{g.Key.Month}-{g.Key.Day} {g.Key.Hour}:00",
                    Count = g.Count()
                })
                .OrderBy(x => x.Time)
                .ToList();

            return new
            {
                levelStats,
                errorStats
            };
        }

        
        public async Task<List<Anomaly>> GetAllAnomaliesAsync()
        {
            return await _context.Anomalies
                .Find(_ => true)
                .SortByDescending(a => a.DetectedAt)
                .ToListAsync();
        }
    }
}