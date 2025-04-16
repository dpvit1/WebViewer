using API.Models;
using API.Repositories;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class FilesService
    {
        private readonly PythonConfig _fileStorageConfig;
        private readonly FileRepository _fileRepository;
    
        public FilesService(IOptions<PythonConfig> fileStorageConfig, FileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _fileStorageConfig = fileStorageConfig.Value;
        }

        public Stream GetFileStream(string fileName)
        {
            var filePath = Path.Combine(_fileStorageConfig.TempFilesDir, fileName);            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {fileName} not found");
            }
            return File.OpenRead(filePath);
        }
    
        public IEnumerable<string> GetList()
        {
            return _fileRepository.GetFileNames();
        }
    
        public void DeleteFile(string fileName)
        {
            _fileRepository.DeleteFile(fileName);
        }
    
        public async Task UploadAsync(Stream fileStream, string fileName)
        {
            await _fileRepository.AddFileAsync(fileStream, fileName);
        }
    }
}
