using API.Models;
using Microsoft.Extensions.Options;
using NoopFormatter = API.Models.NoopFormatter;
using Python.Runtime;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace API.Services;

public class PythonService: IDisposable
{
    private readonly PythonConfig _fileStorageConfig;
    private readonly ConcurrentDictionary<string, Process> _userProcesses;
    public PythonService(IOptions<PythonConfig> fileStorageConfig, IHostApplicationLifetime lifetime)
    {
        lifetime.ApplicationStopping.Register(Dispose);

        RuntimeData.FormatterType = typeof(NoopFormatter);
        _fileStorageConfig = fileStorageConfig.Value;

        _userProcesses = new ConcurrentDictionary<string, Process>();
    }
    public void Dispose()
    {
        foreach (var proc in _userProcesses.Values)
        {
            try
            {
                if (!proc.HasExited) proc.Kill(true);
            }
            catch {}
            finally
            {
                proc.Dispose();
            }
        }
    }

    public async Task<int> RunPythonTaskAsync(string userId, string userCode, string fileName)
    {
        if (_userProcesses.TryGetValue(userId, out var existingProc))
        {
            if (!existingProc.HasExited)
            {
                return -1;
            }
            else
            {
                _userProcesses.TryRemove(userId, out _);
                existingProc.Dispose();
            }
        }

        var fullFileName = fileName + ".gltf";
        var fullFilePathAndName = Path.Combine("./temp/", fullFileName);
        if (File.Exists(fullFilePathAndName))
        {
            File.Delete(fullFilePathAndName);
        }

        var proc = StartWorkerProcess(userId, userCode, fullFilePathAndName);
        _userProcesses[userId] = proc;

        await proc.WaitForExitAsync().ConfigureAwait(false);

        return proc.ExitCode;
    }

    private Process StartWorkerProcess(string userId, string userCode, string fileName)
    {
        string exePath;
        string firstArg;

        var entry = System.Reflection.Assembly.GetEntryAssembly()!.Location;
        exePath = "dotnet";
        firstArg = $"\"{entry}\"";

        string args = $"{firstArg} --worker \"{_fileStorageConfig.PythonHome}\" \"{userCode}\" \"{fileName}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var proc = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        proc.OutputDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"[Worker:{userId}] {e.Data}");
        };
        proc.ErrorDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.Error.WriteLine($"[WorkerError:{userId}] {e.Data}");
        };

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        return proc;
    }
}