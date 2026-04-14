using Microsoft.AspNetCore.Mvc;
using LogPortal.API.Models;
using LogPortal.API.Services;
using System; 

namespace LogPortal.API.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        
        [HttpPost("ingest")]
        public async Task<IActionResult> IngestLog([FromBody] LogEntry log)
        {
            var result = await _logService.IngestLogAsync(log);

            if (result != "Log stored successfully")
                return BadRequest(new { error = result });

            return Ok(new { message = result });
        }

        
        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] string? serverId,
            [FromQuery] string? level,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var logs = await _logService.GetFilteredLogsAsync(
                serverId, level, startDate, endDate, page, pageSize);

            return Ok(logs);
        }

        
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _logService.GetLogStatsAsync();
            return Ok(stats);
        }
    }
}