namespace API.Models
{
    public class PythonConfig
    {
        public required string PythonPath { get; init; }
        public required string TempFilesDir { get; init; }
        public required string FullPathToRgkDist { get; init; }
    }
}
