using API.Dto;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GltfConverterController : ControllerBase
    {
        private readonly ILogger<GltfConverterController> _logger;
        private readonly PythonService _pythonService;

        public GltfConverterController(ILogger<GltfConverterController> logger, PythonService pythonService)
        {
            _logger = logger;
            _pythonService = pythonService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] CodeRequest codeRequest)
        {
            string? userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());
                userId = HttpContext.Session.GetString("UserId");
            }

            int exitCode = await _pythonService.RunPythonTaskAsync(userId, codeRequest.Code, codeRequest.FileName);
            if (exitCode == -1)
            {
                return StatusCode(429);
            }
            else if (exitCode != 0) {
                return StatusCode(500);
            }
            return NoContent();
        }
    }
}
