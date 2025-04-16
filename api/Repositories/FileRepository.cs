using API.Models;
using Microsoft.Extensions.Options;

namespace API.Repositories;

public class FileRepository
{
    private readonly string _tempFilePath;

    public FileRepository(IOptions<PythonConfig> pythonConfig)
    {
        _tempFilePath = pythonConfig.Value.TempFilesDir;
    }
    
    public async Task AddFileAsync(Stream stream, string fileName)
    {
        if (!Directory.Exists(_tempFilePath))
        {
            Directory.CreateDirectory(_tempFilePath);
        }
        var filePath = Path.Combine(_tempFilePath, fileName);
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
    }
    
    public IEnumerable<string> GetFileNames()
    {
        if (!Directory.Exists(_tempFilePath))
        {
            return new List<string>();
        }
        return Directory.GetFiles(_tempFilePath)
            .Select(Path.GetFileName).Where(a => a != null)!;
    }
    
    public void DeleteFile(string fileName)
    {
        var filePath = Path.Combine(_tempFilePath, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}