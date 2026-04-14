using Microsoft.AspNetCore.Mvc;
using LogPortal.API.Services;

namespace LogPortal.API.Controllers
{
    [ApiController]
    [Route("api/anomalies")]
    public class AnomaliesController : ControllerBase
    {
        private readonly LogService _logService;

        public AnomaliesController(LogService logService)
        {
            _logService = logService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllAnomalies()
        {
            var anomalies = await _logService.GetAllAnomaliesAsync();
            return Ok(anomalies);
        }
    }
}