using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<GltfConverterController> _logger;
        private readonly FilesService _filesService;

        public FilesController(ILogger<GltfConverterController> logger, FilesService filesService)
        {
            _logger = logger;
            _filesService = filesService;
        }

        [HttpGet("{fileName}")]
        public IActionResult GetStream(string fileName)
        {
            var gltfFileName = fileName +  ".gltf";
            var stream = _filesService.GetFileStream(gltfFileName);
            return File(stream, "application/octet-stream", gltfFileName);
        }
        
        /// <summary>
        /// Получить список файлов на сервере
        /// </summary>
        [HttpGet("files")]
        public IReadOnlyCollection<string> GetList()
        {
            return _filesService.GetList().ToList();
        }

        /// <summary>
        /// Удалить файл по имени
        /// </summary>
        [HttpDelete("files/{fileName}")]
        public void Delete(string fileName)
        {
            _filesService.DeleteFile(fileName);
        }
        
        /// <summary>
        /// Загрузить файл
        /// </summary>
        /// <returns></returns>
        [HttpPost("files/{fileName}")]
        public async Task Upload(string fileName, IFormFile file)
        {
            await _filesService.UploadAsync(file.OpenReadStream(), fileName);
        }
    }
}
