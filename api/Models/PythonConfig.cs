namespace API.Models
{
    public class PythonConfig
    {
        public required string PythonHome { get; init; }
        public required string TempFilesDir { get; init; }
    }
}
