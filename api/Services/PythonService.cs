using API.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using NoopFormatter = API.Models.NoopFormatter;
using Python.Runtime;

namespace API.Services;

public class PythonService
{
    private readonly PythonConfig _fileStorageConfig;
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
    }

    ~PythonService()
    {
        PythonEngine.Shutdown();
    }

    public void RunPythonScript(string fileName, string userCode)
    {
        var fullFileName = fileName + ".gltf";
        var fullFilePathAndName = Path.Combine(_fileStorageConfig.TempFilesDir, fullFileName);
        if (File.Exists(fullFilePathAndName))
        {
            File.Delete(fullFilePathAndName);
        }

        var mThreadState = PythonEngine.BeginAllowThreads();

        try
        {
            using (Py.GIL())
            {
                string pyPath = PythonEngine.PythonPath;
                string pyHome = PythonEngine.PythonHome;
                string str1 = Path.Combine("C:\\Users\\Admin\\Desktop\\RGKWeb\\RGK_dist\\bin\\RGKGLTF.pyd");
                dynamic rgkGLTFConvertLib = Py.Import(Path.Combine("RGKGLTF"));
                rgkGLTFConvertLib.UserCodeToGLTF(userCode, fullFilePathAndName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            PythonEngine.EndAllowThreads(mThreadState);
        }
    }
}