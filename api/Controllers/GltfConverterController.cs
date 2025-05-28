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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">имя создаваемого файла без расширения</param>
        /// <param name="userCode">Код генерации файла</param>
        /// <remarks>
        /// Sample request:
        ///     RGKPY.Common.Instance.Start()
        ///     session = RGKPY.Common.Session()
        ///     RGKPY.Common.Instance.CreateSession(session)
        ///     context = RGKPY.Common.Context()
        ///     session.CreateMainContext(context)
        ///     side = RGKPY.Math.Vector3D(1, 1, 1)
        ///     lcs = RGKPY.Math.LCS3D()
        ///     data = RGKPY.Generators.PrimitiveGenerator.PrismData(0, 1.0, 1.5, 5, lcs, False, False)
        ///     report = RGKPY.Generators.PrimitiveGenerator.PrismReport()
        ///     result = RGKPY.Generators.PrimitiveGenerator.CreatePrism(context, data, report)
        ///     body1 = report.GetBody()
        ///     data = RGKPY.Generators.PrimitiveGenerator.BoxData(0, lcs, side, False)
        ///     report = RGKPY.Generators.PrimitiveGenerator.BoxReport()
        ///     result = RGKPY.Generators.PrimitiveGenerator.CreateBox(context, data, report)
        ///     body2 = report.GetBody()
        ///     func = RGKPY.Generators.Boolean.Function.Subtract
        ///     data = RGKPY.Generators.Boolean.Data(0, body1, body2, func)
        ///     report = RGKPY.Generators.Boolean.Report()
        ///     result = RGKPY.Generators.Boolean.Run(context, data, report)
        ///     body3 = report.GetBody(0)
        ///     model.append(body3)
        /// </remarks>
        /// <returns>A 204 response code if successful.</returns>
        [HttpPost]
        public ActionResult<string> Create([FromBody] CodeRequest codeRequest)
        {
            string? userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());
                userId = HttpContext.Session.GetString("UserId");
            }

            _pythonService.RunPythonScript(userId, codeRequest.FileName, codeRequest.Code);
            return NoContent();
        }
    }
}
