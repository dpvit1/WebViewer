using API.Models;
using Microsoft.Extensions.Options;
using NoopFormatter = API.Models.NoopFormatter;
using Python.Runtime;

namespace API.Services;

public class PythonService
{
    private readonly PythonConfig _fileStorageConfig;
    private readonly nint mThreadState;
    private dynamic rgkGLTFConvertLib;
    public PythonService(IOptions<PythonConfig> fileStorageConfig)
    {
        RuntimeData.FormatterType = typeof(NoopFormatter);
        _fileStorageConfig = fileStorageConfig.Value;

        string home = _fileStorageConfig.PythonHome;
        Runtime.PythonDLL = Path.Combine(home, "python39.dll");
        PythonEngine.PythonHome = home;
        PythonEngine.PythonPath = string.Join(
            ";",
            $"{home}\\site-packages",
            $"{home}\\DLLs",
            $"{home}"
        );

        PythonEngine.Initialize();
        mThreadState = PythonEngine.BeginAllowThreads();
        using (Py.GIL())
        {
            rgkGLTFConvertLib = Py.Import("RGKGLTF");
        }
        rgkGLTFConvertLib.Init();
    }

    ~PythonService()
    {
        rgkGLTFConvertLib.Shutdown();
        PythonEngine.Shutdown();
        PythonEngine.EndAllowThreads(mThreadState);
    }

    public void RunPythonScript(string userId, string fileName, string userCode)
    {
        var fullFileName = fileName + ".gltf";
        var fullFilePathAndName = Path.Combine("./temp/", fullFileName);
        if (File.Exists(fullFilePathAndName))
        {
            File.Delete(fullFilePathAndName);
        }

        using (Py.GIL())
        {
            try
            {
                rgkGLTFConvertLib.UserCodeToGLTF(userId, userCode, fullFilePathAndName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[PythonException] {ex.Message}\n{ex.StackTrace}");
                throw new Exception(ex.Message);
            }
        }

    }
}